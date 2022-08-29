# TwitterStreamApp
The Twitter API provides a streaming endpoint that delivers a roughly 1% random sample of publicly available Tweets in real-time.

Prerequisites:
- Docker with Linux containers.

Run Project: from the console:
docker compose build
docker compose up

Without docker: 
Need to install RabbitMq software. 
And change appsettitng : 
"Server": "rabbitmq", 
to "Server": "localhost"

Structure:
TwitterStreamV2App : App to collect Twitter stream and publish collected tweets to RabbitMQ message queue.
RabbitMQ : intermediate queue message broker to keep messages in between.
TwitterMassagesConsumerApp : Reads messages from RabbitMq and store in memory. Publish every 50 messages reads and provide statistics.

![image](https://user-images.githubusercontent.com/13110596/184695736-e0b7c54b-93d7-4cad-a53d-6d6f5ee52644.png)

Results:
- Total Tweets count
- Top 10 HashTags 
- Count each HashTag occurrence
- Percentile of HashTag occurrences vs all Tweets received.

How to Scale the app to consume 5700 tweets/second:
- Add load balancing service that will distribute stream between N container with TwitterStreamV2App.
- Use Kubernetes to spawn N containers of TwitterStreamV2App that will push messages into a RabbitMQ or Similar consider replacing with Kafka.
- Spawn N TwitterMassagesConsumerApp in Kubernetes to effectively consume messages and store them to DynamoDB or similar concurrent DB.  

How to make results persistent:
- Each TwitterMassagesConsumerApp will need to save data into a NoSQL database like DynamoDb from AWS.
- NoSQL Db provides a lock implementation that can be used to update concurrent writes. 
- To do that replace the implementation of StorageRepository with a NoSQL one. 

TODO: Add Unit and integration Tests to cover functionality with testing. 
