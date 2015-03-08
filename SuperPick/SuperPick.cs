using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TShockAPI;
using Terraria;
using TerrariaApi.Server;

namespace SuperPick
{
    [ApiVersion( 1,17)]
    public class SuperPick : TerrariaPlugin
    {
        private bool[] players;

        public override string Author
        {
            get { return "Olink"; }
        }

        public override string Description
        {
            get { return "Instant Mining"; }
        }

        public override string Name
        {
            get { return "Super Pick"; }
        }

        public override Version Version
        {
            get { return new Version(1, 0, 0, 0); }
        }

        public SuperPick(Main game)
            : base(game)
        {
            players = new bool[byte.MaxValue];
            for( int i = 0; i < players.Length; i++ )
            {
                players[i] = false;
            }
        }

        public override void Initialize()
        {
            ServerApi.Hooks.ServerLeave.Register(this, OnLeave);
            GetDataHandlers.TileEdit += OnTileEdit;
            Commands.ChatCommands.Add( new Command("superpick", Toggle, "superpick", "sp"));
        }

        private void OnLeave(LeaveEventArgs args)
        {
            players[args.Who] = false;
        }

        private void OnTileEdit( object sender, GetDataHandlers.TileEditEventArgs args )
        {
            if (players[args.Player.Index])
            {
                switch( args.Action )
                {
                    case GetDataHandlers.EditAction.KillTile:
                    case GetDataHandlers.EditAction.KillTileNoItem:
                        WorldGen.KillTile(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                    case GetDataHandlers.EditAction.KillWall:
                        WorldGen.KillWall(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                    case GetDataHandlers.EditAction.KillWire:
                        WorldGen.KillWire(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                    case GetDataHandlers.EditAction.KillWire2:
                        WorldGen.KillWire2(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                    case GetDataHandlers.EditAction.KillWire3:
                        WorldGen.KillWire3(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                }
            }
        }

        private void Toggle( CommandArgs args )
        {
            players[args.Player.Index] = (!players[args.Player.Index]);
            args.Player.SendMessage(String.Format("You have {0} superpick.", (players[args.Player.Index] ? "enabled" : "disabled")), Color.Green);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.ServerLeave.Deregister(this, OnLeave);
                GetDataHandlers.TileEdit -= OnTileEdit;
            }

            base.Dispose(disposing);
        }
    }
}
