[![build-and-test](https://github.com/bartekbiz/PhotoRecall.API/actions/workflows/dotnet.yml/badge.svg)](https://github.com/bartekbiz/PhotoRecall.API/actions/workflows/dotnet.yml)
[![build-and-push-photorecall-api](https://github.com/bartekbiz/PhotoRecall.API/actions/workflows/docker-publish.yml/badge.svg)](https://github.com/bartekbiz/PhotoRecall.API/actions/workflows/docker-publish.yml)
[![build-and-push-photorecall-api-dev](https://github.com/bartekbiz/PhotoRecall.API/actions/workflows/docker-publish-dev.yml/badge.svg)](https://github.com/bartekbiz/PhotoRecall.API/actions/workflows/docker-publish-dev.yml)

# PhotoRecall.API

## Project Overview
**PhotoRecall.API** is a component of the larger **PhotoRecall** project, which aims to develop an intelligent gallery app. 
This **RESTful** API serves the **PhotoRecall.Frontend** user app, providing access to various YOLO (You Only Look Once) algorithms for object recognition in photos.
**PhotoRecall.API** processes incoming photos by sending them to the [YoloRunner](https://github.com/bartekbiz/PhotoRecall.YoloRunner/) services. 
Based on user preferences, the API can either merge the predictions from different YOLO models through a voting mechanism or 
return each model's results individually. 
In the future, additional methods for merging predictions may be implemented to further enhance the API's functionality.

## How it works?
This API uses [OpenTelemetry](https://opentelemetry.io/) for logging, which requires a **Seq** container to run concurrently for log collection. 
Additionally, the API needs at least one instance of [YoloRunner](https://github.com/bartekbiz/PhotoRecall.YoloRunner/) to generate predictions.
Each YoloRunner instance should be specified in the `appsettings.json` file, including the URLs at which they are accessible and the models they support. 
There is no limit to the number of YoloRunner instances or replicas that can be connected, and they operate in parallel to ensure fast prediction processing.
A ready-to-use `appsettings.json` and `docker-compose` examples are provided, requiring no configuration from the user.

## Installation
Firstly, you need to have [Docker](https://www.docker.com/get-started/) installed, next follow these steps to set up the containers:

1. Clone the repository:
```sh
git clone https://github.com/bartekbiz/PhotoRecall.API.git
cd PhotoRecall.API
```

2. Copy `appsettings.json` to `config` directory:
```sh
mkdir config
cp config-example/appsettings.json config/
```

3. Start the containers using Docker Compose:
```sh
docker compose up -d
```

## Building Docker images
You can build the Docker image yourself.
```sh
docker build --tag 'photorecall-api' -f Dockerfile .
```

## Available endpoints
Documentation for the available endpoints is provided via **Swagger** and can be accessed by adding `/swagger/index.html` to the API's URL:

For example, in a development environment:
```
http://localhost:5075/swagger/index.html
```

## Getting Predictions

To request predictions, use the appropriate endpoints, providing a photo and, optionally, a list of YOLO models in the request body as `multipart/form-data`.

### Example: Voted Predictions
This example retrieves voted predictions using the models `["yolo11x.pt", "yolov8x.pt", "yolov7-d6.pt"]` for the photo `bus.jpg`:

```bash
curl -X 'POST' \
  'http://localhost:5075/api/Predictions/GetVotedPredictionsWithCountAsync?YoloModels=%5B%22yolo11x.pt%22%2C%22yolov8x.pt%22%2C%22yolov7-d6.pt%22%5D' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'Photo=@bus.jpg;type=image/jpeg'
```

### Example: All Available Models
This example retrieves predictions using all available models for the photo `bus.jpg`:

```bash
curl -X 'POST' \
  'http://localhost:5075/api/Predictions/GetAllPredictionsAsync' \
  -H 'accept: */*' \
  -H 'Content-Type: multipart/form-data' \
  -F 'photo=@bus.jpg;type=image/jpeg'
```

## Reading Results
Example results from endpoint `/api/Predictions/GetVotedPredictionsWithCountAsync`:
```json
[
    {
        "count": 2,          // Count of detected item
        "name": "laptop"     // YOLO class of detected item
    },
    {
        "count": 1,
        "name": "bench"
    },
    {
        "count": 15,
        "name": "person"
    },
    {
        "count": 14,
        "name": "chair"
    },
    {
        "count": 1,
        "name": "tie"
    },
    {
        "count": 1,
        "name": "cup"
    }
]
```

Example results from endpoint `/api/Predictions/GetAllPredictionsAsync`:
```json
[                              // List of predictions from each YOLO model
    {
        "yoloRunnerInfo": {    // Info about YoloRunner container and used model
            "name": "YoloUltralyticsRunner",  
            "url": "http://yolo_ultralytics_runner-1:8000",
            "model": "yolo11x.pt"
        },
        "predictions": [       // List of predictions in normalized format
            {
                "class": 5,
                "confidence": 0.93498,
                "box": {
                    "x1": 7.17651,
                    "y1": 230.97098,
                    "x2": 805.10663,
                    "y2": 736.83911
                },
                "name": "bus"
            },
            {
                "class": 0,
                "confidence": 0.79012,
                "box": {
                    "x1": 0.17425,
                    "y1": 549.95343,
                    "x2": 79.01257,
                    "y2": 871.94537
                },
                "name": "person"
            }
        ]
    },
    {
        "yoloRunnerInfo": {
            "name": "Yolo7Runner",
            "url": "http://yolo_7_runner:8000",
            "model": "yolov7-d6.pt"
        },
        "predictions": [
            {
                "class": 0,
                "confidence": 0.624811,
                "box": {
                    "x1": -4E-05,
                    "y1": 485.00046,
                    "x2": 77.99996,
                    "y2": 872.0001
                },
                "name": "person"
            },
            {
                "class": 5,
                "confidence": 0.954236,
                "box": {
                    "x1": 1.99949,
                    "y1": 227.99988,
                    "x2": 805.99982,
                    "y2": 735.99948
                },
                "name": "bus"
            }
        ]
    }
]
```

## License
GNU General Public License v3.0 or later.
See LICENSE to see the full text.
