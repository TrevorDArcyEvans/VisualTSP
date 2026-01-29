{
  "Id": "bd75bd0b-b9d4-4fe2-b83a-fe9cb9662ed8",
  "Name": "hub-spoke",
  "Nodes": [
    {
      "Top": 313,
      "Left": 372,
      "Node": {
        "Id": "ba56be1c-6bd7-4564-8652-e1779fa6f8fb",
        "Name": "hub"
      }
    },
    {
      "Top": 194,
      "Left": 206,
      "Node": {
        "Id": "eefe639d-3acc-4c7f-a8b5-09a0e7ea0347",
        "Name": "start"
      }
    },
    {
      "Top": 149,
      "Left": 418,
      "Node": {
        "Id": "efd2b8e1-1a11-4e54-a7a4-e6ee846c9a6c",
        "Name": "aaa"
      }
    },
    {
      "Top": 303,
      "Left": 582,
      "Node": {
        "Id": "bce134c5-8f3c-4869-ad26-ff7eaece9f25",
        "Name": "bbb"
      }
    },
    {
      "Top": 503,
      "Left": 388,
      "Node": {
        "Id": "a73fde3c-cb48-4992-81db-3b4971f52591",
        "Name": "end"
      }
    }
  ],
  "Links": [
    {
      "X1": 397,
      "Y1": 338,
      "X2": 231,
      "Y2": 219,
      "Link": {
        "Id": "d337691b-2f32-4995-8fdc-967bbb11cc3e",
        "Start": "ba56be1c-6bd7-4564-8652-e1779fa6f8fb",
        "End": "eefe639d-3acc-4c7f-a8b5-09a0e7ea0347",
        "Cost": 10
      }
    },
    {
      "X1": 397,
      "Y1": 338,
      "X2": 443,
      "Y2": 174,
      "Link": {
        "Id": "cd991cfa-da40-40f0-b90d-d8d61d7bf3cb",
        "Start": "ba56be1c-6bd7-4564-8652-e1779fa6f8fb",
        "End": "efd2b8e1-1a11-4e54-a7a4-e6ee846c9a6c",
        "Cost": 10
      }
    },
    {
      "X1": 607,
      "Y1": 328,
      "X2": 397,
      "Y2": 338,
      "Link": {
        "Id": "341bb26a-585a-44b1-8035-e5f6fb544378",
        "Start": "bce134c5-8f3c-4869-ad26-ff7eaece9f25",
        "End": "ba56be1c-6bd7-4564-8652-e1779fa6f8fb",
        "Cost": 10
      }
    },
    {
      "X1": 413,
      "Y1": 528,
      "X2": 397,
      "Y2": 338,
      "Link": {
        "Id": "3efaf537-0c8a-4b81-94ad-590eb63b065b",
        "Start": "a73fde3c-cb48-4992-81db-3b4971f52591",
        "End": "ba56be1c-6bd7-4564-8652-e1779fa6f8fb",
        "Cost": 10
      }
    }
  ],
  "Start": "eefe639d-3acc-4c7f-a8b5-09a0e7ea0347",
  "End": "a73fde3c-cb48-4992-81db-3b4971f52591"
}