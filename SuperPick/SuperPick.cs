using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hooks;
using TShockAPI;
using Terraria;

namespace SuperPick
{
    [APIVersion( 1,12)]
    public class SuperPick : TerrariaPlugin
    {
        public struct player
        {
            public player(bool toggle, bool recurve)
            {
                this.toggle = toggle;
                this.recurve = recurve;
            }
            public bool toggle;
            public bool recurve;
        }
        public player[] players;

        public override string Author
        {
            get { return "Zack"; }
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
            players = new player[255];
            player normal = new player(false, false);
            for( int i = 0; i < players.Length; i++ )
            {
                players[i] = normal;
            }
        }

        public override void Initialize()
        {
            Hooks.ServerHooks.Leave += OnLeave;
            GetDataHandlers.TileEdit += OnTileEdit;
            Commands.ChatCommands.Add( new Command("superpick", Toggle, "superpick", "sp"));
            Commands.ChatCommands.Add(new Command("recursivepick", RecurveToggle, "recursive"));
        }

        private void OnLeave( int who )
        {
            players[who].toggle = false;
            players[who].recurve = false;
        }

        private void OnTileEdit( object sender, GetDataHandlers.TileEditEventArgs args )
        {
            if (players[args.Player.Index].toggle)
            {
                switch (args.EditType)
                {
                    case 0:
                    case 4:
                        WorldGen.KillTile(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                    case 2:
                        WorldGen.KillWall(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                    case 6:
                        WorldGen.KillWire(args.X, args.Y);
                        TSPlayer.All.SendTileSquare(args.X, args.Y, 1);
                        break;
                }
            }
            else
            {
                if (players[args.Player.Index].recurve)
                {
                    switch (args.EditType)
                    {
                        case 0:
                        case 4:
                            {
                                Recursive re = new Recursive();
                                List<Vector2> del = re.RecursiveEdit(args.X, args.Y, args.EditType);
                                foreach (Vector2 tip in del)
                                {
                                    WorldGen.KillTile(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y));
                                    TSPlayer.All.SendTileSquare(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y), 1);
                                }
                                break;
                            }
                        case 2:
                            {
                                Recursive re = new Recursive();
                                List<Vector2> del = re.RecursiveEdit(args.X, args.Y, args.EditType);
                                foreach (Vector2 tip in del)
                                {
                                    WorldGen.KillWall(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y));
                                    TSPlayer.All.SendTileSquare(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y), 1);
                                }
                                break;
                            }
                        case 6:
                            {
                                Recursive re = new Recursive();
                                List<Vector2> del = re.RecursiveEdit(args.X, args.Y, args.EditType);
                                foreach (Vector2 tip in del)
                                {
                                    WorldGen.KillWire(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y));
                                    TSPlayer.All.SendTileSquare(Convert.ToInt16(tip.X), Convert.ToInt16(tip.Y), 1);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private void Toggle( CommandArgs args )
        {
            players[args.Player.Index].toggle = (!players[args.Player.Index].toggle);
            args.Player.SendMessage(String.Format("You have {0} superpick.", (players[args.Player.Index].toggle ? "enabled" : "disabled")), Color.Green);
        }

        private void RecurveToggle(CommandArgs args)
        {
            players[args.Player.Index].recurve = (!players[args.Player.Index].recurve);
            args.Player.SendMessage(String.Format("You have {0} recursivepick.", (players[args.Player.Index].recurve ? "enabled" : "disabled")), Color.Green);
        }
       
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerHooks.Leave -= OnLeave;
                GetDataHandlers.TileEdit -= OnTileEdit;
            }

            base.Dispose(disposing);
        }
    }
}
