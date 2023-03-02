# Microsoft.Graph.Batching

[![Language][badge_language]][link_repo]
[![License][badge_license]][link_repo]
[![github issues][badge_issues]][link_issues]
[![github stars][badge_repo_stars]][link_repo]
[![github sponsor][badge_sponsor]][link_sponsor]

The Microsoft Graph Client [has](https://learn.microsoft.com/en-us/graph/sdks/batch-requests?tabs=csharp) support for Batching, which is a great idea when you are doing a lot of requests to the Graph API. By batching requests you can achieve much higher throughput.

The original batch implementation in the GraphServiceClient feels incomplete, by default the GraphServiceClient let's you combine up to 20 requests before throwing an exception.

By using this [extension](#introducing-the-batchrequestcontentcollection) you can combine "unlimited" requests and have this library automatically split up the requests in multiple batches. While staying very close to the original implementation.

[![Hackathon][badge_hackathon]][link_hackathon]

This project was starting during the [Hack Together: Microsoft Graph and .NET][link_hackathon]

## Batching with Microsoft Graph

This library stays really close to the [build-in batch support](https://learn.microsoft.com/en-us/graph/sdks/batch-requests?tabs=csharp) so go ahead and read that documentation before hand.

```csharp
// Create a GraphServiceClient with your required IAuthenticationProvider
var graphClient = new GraphServiceClient(...);

// Create a BatchRequestContent (your batch request container)
var batchRequestContent = new BatchRequestContent(graphClient);

// Add two or more (but max 20) requests to it
var getRequest1 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
var getRequest2 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());

// Execute the batch request
var response = await graphClient.Batch.PostAsync(batchRequestContent);

// Do something with the result
var user = await response.GetResponseByIdAsync<User>(getRequest1);
Console.WriteLine("Hi {0}", user.DisplayName);
```

## Introducing the BatchRequestContentCollection

Instead of creating a **BatchRequestContent**, you now create a **BatchRequestContentCollection** and continue using it as before.

```csharp
// Create a GraphServiceClient with your required IAuthenticationProvider
var graphClient = new GraphServiceClient(...);

// Create a BatchRequestContentCollection (your batch request container)
var batchRequestContent = new BatchRequestContentCollection(graphClient);

// Add two or more requests to it
// If you add more then 20 they will be spitted across multiple batch requests automatically.
var getRequest1 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());
var getRequest2 = await batchRequestContent.AddBatchRequestStepAsync(graphClient.Me.ToGetRequestInformation());

// Execute all the batch requests
var response = await graphClient.Batch.PostAsync(batchRequestContent);

// Do something with the result
var user = await response.GetResponseByIdAsync<User>(getRequest1);
...
```

## Things to keep in mind

- You **cannot** combine requests to multiple tenants in a single batch.
- You **cannot** combine requests to `beta` and `v1` endpoints.
- You should test wether or not batching results in higher speeds.

Regular batching support request dependencies, because you don't know if the requests are put in the same batch, you should be careful depending on those.

## About the author

[![LinkedIn Profile][badge_linkedin]][link_linkedin]
[![Link Mastodon][badge_mastodon]][link_mastodon]
[![Follow on Twitter][badge_twitter]][link_twitter]
[![Check my blog][badge_blog]][link_blog]

I like building applications and am somewhat of a Microsoft Graph API expert. I used this knowledge to build this batching helper. But I'm only human so please validate, and if you find an [issue][link_issues] please let me know. If you like this extension give me a shout out on [twitter @svrooij][link_twitter].

[badge_hackathon]: https://img.shields.io/badge/Microsoft%20365-Hackathon-orange?style=for-the-badge&logo=microsoft
[link_hackathon]: https://github.com/microsoft/hack-together

[badge_blog]: https://img.shields.io/badge/blog-svrooij.io-blue?style=for-the-badge
[badge_linkedin]: https://img.shields.io/badge/LinkedIn-stephanvanrooij-blue?style=for-the-badge&logo=linkedin
[badge_mastodon]: https://img.shields.io/mastodon/follow/109502876771613420?domain=https%3A%2F%2Fdotnet.social&label=%40svrooij%40dotnet.social&logo=mastodon&logoColor=white&style=for-the-badge
[badge_twitter]: https://img.shields.io/badge/follow-%40svrooij-1DA1F2?logo=twitter&style=for-the-badge&logoColor=white
[link_blog]: https://svrooij.io/
[link_linkedin]: https://www.linkedin.com/in/stephanvanrooij
[link_mastodon]: https://dotnet.social/@svrooij
[link_twitter]: https://twitter.com/svrooij

[badge_language]: https://img.shields.io/badge/language-C%23-blue?style=for-the-badge
[badge_license]: https://img.shields.io/github/license/svrooij/msgraph-sdk-dotnet-batching?style=for-the-badge
[badge_issues]: https://img.shields.io/github/issues/svrooij/msgraph-sdk-dotnet-batching?style=for-the-badge
[badge_repo_stars]: https://img.shields.io/github/stars/svrooij/msgraph-sdk-dotnet-batching?logo=github&style=for-the-badge
[badge_sponsor]: https://img.shields.io/github/sponsors/svrooij?logo=github&style=for-the-badge
[link_issues]: https://github.com/svrooij/msgraph-sdk-dotnet-batching/issues
[link_repo]: https://github.com/svrooij/msgraph-sdk-dotnet-batching
[link_sponsor]: https://github.com/sponsors/svrooij