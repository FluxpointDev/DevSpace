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

![image](https://github.com/user-attachments/assets/37493e01-4cf0-4add-a6f8-2429bf66fb71)


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

---

### Current/Planned Features
2FA will be required for all users that register in your instance (self-host)

Support for Email, Authenticator app, Passkeys and Recovery code has already been added.
![image](https://github.com/user-attachments/assets/53e167bb-50e4-44a9-ba4d-6f15a29da24b)

- Server/VPS management with docker containers, firewall, files, status, database and webhooks.
- Image manipulation and generation tools with different formats webp, resize and convert or generate banners/templates. 
- Website management with cloudflare support and network tools.
- Project management with todo/kanban boards, internal documentation to share with other developers/users, notes/memos and other related tools
- Error logging which will use the Sentry protocol for a drop-in replacement and linked projects/websites
- Status monitoring which will check the uptime of your servers, websites, projects and also process checking directly.

---

### Inspiration/Ideas
I will be taking ideas from other similar developer tools such as:
- [Sentry](https://sentry.io/welcome/)
- [Portainer](https://www.portainer.io/)
- [Pterodactyl](https://pterodactyl.io)
- [HetrixTools](https://hetrixtools.com)
- [DnsChecker](https://dnschecker.org)
- [Coolify](https://coolify.io)

---

### Other Screenshots
![image](https://github.com/user-attachments/assets/7cd35fbf-c721-4980-b2e5-bb843cabb2cb)

![image](https://github.com/user-attachments/assets/cfb41757-85d6-4343-aafe-c2d446c9962b)

![image](https://github.com/user-attachments/assets/497ac1e4-2818-455b-885f-e98ba090ac6d)
