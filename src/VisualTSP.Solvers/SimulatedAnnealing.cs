namespace VisualTSP.Solvers;

using System.Numerics;

public sealed class SimulatedAnnealing
{
  private readonly Vector2[] _cities;
  private float _currentDistance;
  private float _temperature;

  private int _iterationCount;

  public readonly float TemperatureDecay;

  public readonly bool ReheatWhenCool = false;
  public readonly float ReheatThresholdTemperature;
  public readonly float MinReheatAmount;
  public readonly float MaxReheatAmount;

  public Vector2[] BestRoute => _cities;
  public float BestDistance => _currentDistance;
  public float CurrentTemperature => _temperature;
  public int IterationCount => _iterationCount;

  public SimulatedAnnealing(
    Vector2[] cities,
    float initialTemperature = 100f,
    float temperatureDecay = 0.99f,
    bool reheatWhenCool = false,
    float reheatThresholdTemperature = 0.005f,
    float minReheatAmount = 0.5f,
    float maxReheatAmount = 20)
  {
    _cities = new Vector2[cities.Length];
    Array.Copy(cities, _cities, cities.Length);

    // initialise to a distance from a simple algorithm
    _currentDistance = float.MaxValue;

    _temperature = initialTemperature;
    TemperatureDecay = temperatureDecay;

    ReheatWhenCool = reheatWhenCool;
    ReheatThresholdTemperature = reheatThresholdTemperature;
    MinReheatAmount = minReheatAmount;
    MaxReheatAmount = maxReheatAmount;

    _iterationCount = 0;
  }

  private void SwapCities(int idx1, int idx2)
  {
    (_cities[idx1], _cities[idx2]) = (_cities[idx2], _cities[idx1]);
  }

  private int WrapIndex(int idx) => ((idx % _cities.Length) + _cities.Length) % _cities.Length;

  private int RandomIndex() => Random.Shared.Next(_cities.Length);

  private void ReverseRange(int startIndex, int count)
  {
    for (var i = 0; i <= count / 2; i++)
    {
      var left = WrapIndex(startIndex + i);
      var right = WrapIndex(startIndex + count - i);
      SwapCities(left, right);
    }
  }

  private void TransportRange(int startIndex, int count, int distance)
  {
    var citiesToMove = new Vector2[count];
    for (var i = 0; i < count; i++)
    {
      citiesToMove[i] = _cities[WrapIndex(startIndex + i)];
    }

    // Move the right segment to the left.
    for (var i = 0; i < distance; i++)
    {
      _cities[WrapIndex(startIndex + i)] = _cities[WrapIndex(startIndex + i + count)];
    }

    // Move the previous left segment to the right.
    for (var i = 0; i < count; i++)
    {
      _cities[WrapIndex(startIndex + distance + i)] = citiesToMove[i];
    }
  }

  private float ComputeDistanceDeltaAfterReverse(int startIndex, int count)
  {
    var endIndex = WrapIndex(startIndex + count);

    var positionBeforeStart = _cities[WrapIndex(startIndex - 1)];
    var startPosition = _cities[startIndex];
    var endPosition = _cities[endIndex];
    var positionAfterEnd = _cities[WrapIndex(endIndex + 1)];

    // When reversing a range of cities, the distances between the individual
    // cities remain the same. The only thing that changes are the distances between
    // the start and end positions to their predecessor and successor, respectively.

    return 0
           - positionBeforeStart.DistanceTo(startPosition)
           - endPosition.DistanceTo(positionAfterEnd)
           + positionBeforeStart.DistanceTo(endPosition)
           + startPosition.DistanceTo(positionAfterEnd);
  }

  private float ComputeDistanceDeltaAfterSwap(int indexA, int indexB)
  {
    var indexBeforeA = WrapIndex(indexA - 1);
    var indexBeforeB = WrapIndex(indexB - 1);
    var indexAfterA = WrapIndex(indexA + 1);
    var indexAfterB = WrapIndex(indexB + 1);

    var posBeforeA = _cities[indexBeforeA];
    var posA = _cities[indexA];
    var posAfterA = _cities[indexAfterA];

    var posBeforeB = _cities[indexBeforeB];
    var posB = _cities[indexB];
    var posAfterB = _cities[indexAfterB];

    float delta = 0;
    delta -= posBeforeA.DistanceTo(posA);
    delta -= posA.DistanceTo(posAfterA);

    delta -= posBeforeB.DistanceTo(posB);
    delta -= posB.DistanceTo(posAfterB);

    // Positions of predecessors / successors may change due to the swap.
    posBeforeA = indexBeforeA == indexB ? posA : posBeforeA;
    posBeforeB = indexBeforeB == indexA ? posB : posBeforeB;
    posAfterA = indexAfterA == indexB ? posA : posAfterA;
    posAfterB = indexAfterB == indexA ? posB : posAfterB;

    delta += posBeforeA.DistanceTo(posB);
    delta += posB.DistanceTo(posAfterA);

    delta += posBeforeB.DistanceTo(posA);
    delta += posA.DistanceTo(posAfterB);

    return delta;
  }

  private float ComputeDistanceDeltaAfterTransport(int startIndex, int count, int distance)
  {
    var leftSegmentStartIndex = startIndex;
    var leftSegmentEndIndex = WrapIndex(startIndex + count - 1);
    var indexBeforeLeftSegment = WrapIndex(startIndex - 1);

    var rightSegmentStartIndex = WrapIndex(leftSegmentEndIndex + 1);
    var rightSegmentEndIndex = WrapIndex(rightSegmentStartIndex + distance - 1);
    var indexAfterRightSegment = WrapIndex(rightSegmentEndIndex + 1);

    var posBeforeLeftSegment = _cities[indexBeforeLeftSegment];
    var leftSegmentStart = _cities[leftSegmentStartIndex];
    var leftSegmentEnd = _cities[leftSegmentEndIndex];

    var rightSegmentStart = _cities[rightSegmentStartIndex];
    var rightSegmentEnd = _cities[rightSegmentEndIndex];
    var posAfterRightSegment = _cities[indexAfterRightSegment];

    float delta = 0;
    delta -= posBeforeLeftSegment.DistanceTo(leftSegmentStart);
    delta -= leftSegmentEnd.DistanceTo(rightSegmentStart);
    delta -= rightSegmentEnd.DistanceTo(posAfterRightSegment);

    delta += posBeforeLeftSegment.DistanceTo(rightSegmentStart);
    delta += rightSegmentEnd.DistanceTo(leftSegmentStart);
    delta += leftSegmentEnd.DistanceTo(posAfterRightSegment);

    return delta;
  }

  public void Simulate()
  {
    if (ReheatWhenCool && _temperature < ReheatThresholdTemperature)
    {
      _temperature += (float)(Random.Shared.NextDouble() * (MaxReheatAmount - MinReheatAmount) + MinReheatAmount);
    }

    if (_cities.Length <= 3)
    {
      _temperature *= TemperatureDecay;
      _iterationCount += 1;
      return;
    }

    Action acceptSolution = () => throw new NotImplementedException();
    float distanceChange = 0;

    switch (Random.Shared.Next(6))
    {
      case 0:
        var swapIndexA = RandomIndex();
        var swapIndexB = RandomIndex();

        acceptSolution = () => SwapCities(swapIndexA, swapIndexB);
        distanceChange = ComputeDistanceDeltaAfterSwap(swapIndexA, swapIndexB);
        break;

      case 1:
      case 2:
        // This operation only works for more than 3 cities.
        var startIndex = RandomIndex();
        var count = Random.Shared.Next(1, _cities.Length / 4);
        // Note: count+distance must be LESS than the number of cities for the
        // ComputeDistanceDeltaAfterTransport method to work properly.
        var distance = Random.Shared.Next(1, _cities.Length - count);

        acceptSolution = () => TransportRange(startIndex, count, distance);
        distanceChange = ComputeDistanceDeltaAfterTransport(startIndex, count, distance);
        break;

      // Twice as likely as it is more powerful.
      case 3:
      case 4:
      case 5:
        var reverseStartIndex = RandomIndex();
        var reverseCount = Random.Shared.Next(1, _cities.Length / 2);

        acceptSolution = () => ReverseRange(reverseStartIndex, reverseCount);
        distanceChange = ComputeDistanceDeltaAfterReverse(reverseStartIndex, reverseCount);
        break;
    }

    if (distanceChange <= 0)
    {
      _currentDistance += distanceChange;
      acceptSolution();
    }
    else if (MathF.Exp(-distanceChange / _temperature) >= Random.Shared.NextSingle())
    {
      _currentDistance += distanceChange;
      acceptSolution();
    }

    _temperature *= TemperatureDecay;
    _iterationCount += 1;
  }
}

internal static class Vector2Extensions
{
  public static float DistanceTo(this Vector2 v1, Vector2 v2) => Vector2.Distance(v1, v2);
}
