# Day03

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
