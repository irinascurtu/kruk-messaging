# Hypermedia

Now, lets assume that we want to expose our API to a specific client.
We want to make sure that we don't deploy a new version of the API every time we add a new feature,
and we can accoomodate several clients with different needs.

	-  the easiest way of doing that is to give each client a different endpoint, but that is not scalable.
	-  we can also use query parameters to filter the data, but that is not very flexible.
	- we can also use headers to specify the version of the API, but that is not very flexible either.
	- we can also use hypermedia links to navigate through the API.
	- and we can use different media types to represent the data.

and we want to make it easier for the client to navigate through the API. We can do this by adding hypermedia links to our API.
- add a `Consumes` to consume media types
-  