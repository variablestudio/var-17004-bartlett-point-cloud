# Day03

## Assignment
- scene in unity using what we had learned
- 2 data sources that influence geometry (scale, height, stream)
- time label
- spreadsheet describes the data 
  - what's the source ( which website, which data set / spreadsheet, what API)
  - what is the data (e.g. no2 = nitrogen dioxide, count = number of people on the bus stop)
  - value range (0..1? 10-100? 1000s)
  - time range (24h? 4 week? one week?)
- output format = ??? unity files > screenshot > video > ? (before 13th)
- what do we do with the point cloud???????????

## Evaluation
- what date have you used and how much research have you done?
- how did you use the primitives to describe the data?
- visual quality = readability
- documentation
- how much have you challenged yourself
- deadline 20th

## Agenda

- data stream updates (radius, trasnparency)
- data label
- live data Air Quality
- adding point clouds again
- feedback
- assignment and evaluation criteria

## Air Quality

Hackney - Old Street Air Quality
http://aqicn.org/city/united-kingdom/hackney-old-street/

Air Quality API
http://aqicn.org/api/

Requesting an API Token
http://aqicn.org/data-platform/token/

API Test Page
http://aqicn.org/json-api/demo/

Find station ID
http://api.waqi.info/search/?keyword=Hackney&token=TOKEN

```json
{"status":"ok","data":[{"uid":7946,"aqi":"37","time":{"tz":"+0000","stime":"2017-02-24 06:00:00","vtime":1487916000},"station":{"name":"Hackney - Old Street, United Kingdom","geo":[51.526454,-0.08491],"url":"united-kingdom/hackney-old-street"}}]}
```

Hackney Old Street ID = 7946

Get the data
http://api.waqi.info/feed/@7946/?token=TOKEN

```json
{"status":"ok","data":{"aqi":37,"idx":7946,"attributions":[{"url":"http://uk-air.defra.gov.uk/","name":"UK-AIR, air quality information resource - Defra, UK"},{"url":"http://londonair.org.uk/","name":"London Air Quality Network - Environmental Research Group, King's College London"}],"city":{"geo":[51.526454,-0.08491],"name":"Hackney - Old Street","url":"http://aqicn.org/city/united-kingdom/hackney-old-street/"},"dominentpol":"pm25","iaqi":{"co":{"v":0.1},"h":{"v":80},"no2":{"v":27.3},"o3":{"v":4.4},"p":{"v":1014},"pm10":{"v":12},"pm25":{"v":37},"so2":{"v":0.2},"t":{"v":2.55}},"time":{"s":"2017-02-24 06:00:00","tz":"+00:00","v":1487916000}}}
```

Format the data
http://jsonformatter.org

## Used libraries

https://lbv.github.io/litjson/
