## Dev Space
Most developer tools can be clunky to use, needs a lot of ram or lacks basic features such as.
- Sentry - Requires a minimum of 6GB ram with microservice hell
- Portainer - Very basic and confusing user access with paid self-hosting licenses

Dev Space is a self-hostable and easy to use dashboard that allows you to manage your VPS servers, websites, projects and other services and is 100% free and open source!

This is written in C# using asp.net blazor and various other frameworks, libs and tools which will be much better in terms of ram usage too.

> [!WARNING]  
> This project is currently WIP and should not be used until a release is confirmed.
> 
> If you fork or pull this repo i may not offer support if something breaks unless you use the release version.

---

### How to Contribute?
The project is currently WIP so any kind of code contributions wont really be accepted but you can use the Discussion page to suggest features, alternative services that we could merge features from or general questions about the project.

There is also a project board here https://github.com/orgs/FluxpointDev/projects/7 with todo or in progress features for the project.

You can also join the [Fluxpoint Discord Server](https://discord.gg/fluxpoint.dev)

---

### How does this work?
This project uses .net 8 with asp.net blazor web server with SSR (server-side rendering) and MongoDB for the database.

The website will be self-hostable on any linux server.

You can then install the Agent service on your linux server (currently ubuntu supported) which will remotely manage your server using a websocket communication to the dashboard.
