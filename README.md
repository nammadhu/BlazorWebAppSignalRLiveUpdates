# BlazorWebAppSignalR

This is To Help Users to use SignalR with Blazor 
And in combination of localstorage, api, delta fetch and progressive quick updates in an effective manner

Multiple examples are in different branches of the repo in an incremental manner for beginners to go step by step.
like Ex1 ,Ex2 ...


First Time,
OnInitializedAsync only confuigured SignalR()
on first time load,
if fetch from localstorage exists use+ Process Delta Updates(API response) 
else fetch full (API response)
Then store on cache local storage

SingalR configured as,
when joined first time,signalR can be used but its taking time so instead Process Delta Updates(API response) 
 to make sure
 
Then whenever any new cards added/updated will Broadcast and deliver to clients


For Proper timings handle used LastSyncTime LastUpdatedTime  on server hubs and client
