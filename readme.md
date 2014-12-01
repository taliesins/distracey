Distracey
=========

# What

Easily expose Application Performance Metrics and logging for WebApi Controllers.

# Why

When running micro-services architecture it is really important to monitor and log every website involved as transactions are spanned over multiple websites which makes it difficult to debug and trace. With good monitoring and logging it alleviates the major drawback of micro-services.

# How

## Architecture

* Adds a global action filter which will record all incoming requests and reponses on the webapi controllers. 
* Exposes an http delegating handler which will record outgoing requests and responses with the http client. 

## Architecture

# Logging and metrics targets

* Performance counters
** Average time taken
** Last operation execution time
** Number of operations per second
** Total count handler
* Logary (https://github.com/logary/logary)
** Soon Logary Zipkin target will be complete and then you can view distributed tracing and metrics for Web Api controllers via ZipKin.
* Log4Net (https://github.com/apache/log4net)
** Add Log4Net GELF target (https://github.com/jjchiw/gelf4net) and log to logstash configured with an output to elastic search. Then use kibana for a great dashboard experince.

# How to install

Pull in nuget package for the target you want to use. You can use all the targets if you want to.

* Distracey.PerformanceCounters
* Distracey.Logary
* Distracey.Log4Net

# Acknowledgments

* Used a lot of ideas from Ali https://github.com/aliostad/PerfIt and http://byterot.blogspot.co.uk/2013/04/Monitor-your-ASP-NET-Web-API-application-using-your-own-custom-counters.html
* Mono reflection https://github.com/jbevain/mono.reflection/tree/master/Mono.Reflection
* ShortGuid http://www.singular.co.nz/2007/12/shortguid-a-shorter-and-url-friendly-guid-in-c-sharp/