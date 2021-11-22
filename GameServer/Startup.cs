using GameServer.Game;
using GameServer.Hubs;
using GameServer.States;
using GameServer.World;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace GameServer
{
    public class Startup
    { 
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSignalR();
            services.AddSingleton<IGameEngine, GameEngine>();
            services.AddSingleton<IConfig, Config>();
            services.AddSingleton<IMapState, MapState>();
            services.AddSingleton<IWorldLoader, WorldLoader>();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseFileServer();

            app.UseSignalR(routes =>
            {
                routes.MapHub<GameHub>("/game");
            });

            app.UseStaticFiles();
        }
    }
}

