﻿using System;
using System.Collections.Generic;
using System.Linq;
using MapleLib.WzLib;
using MapleLib.WzLib.WzProperties;
using MapleLib.WzLib.WzStructure.Data;
using NineToFive.Constants;
using NineToFive.Game;
using NineToFive.Game.Entity.Meta;

namespace NineToFive.Wz {
    public static class MapWz {
        private const string WzName = "Map";
        
        /// <summary>
        /// Sets the the field variables of the field being passed in.
        /// </summary>
        /// <param name="Field">Field to be initialized</param>
        /// <param name="MapProperties">List of WzImageProperty loaded from the Field's Image from Wz.</param>
        public static void SetField(Field Field, ref List<WzImageProperty> MapProperties) {
            if (Field == null || MapProperties == null) return;
            
            Dictionary<uint, object> TemplateFields = Server.Worlds[0].Templates[(int) TemplateType.Field];
            if (!TemplateFields.TryGetValue(Field.ID, out object Template)) {
                Template = new TemplateField();
                SetTemplateField((TemplateField) Template, ref MapProperties);
                TemplateFields.Add(Field.ID, Template);
            }

            if (Template == null) return;
            Field.Properties = (TemplateField) Template;
        }

        /// <summary>
        /// Sets the Template of the field.
        /// </summary>
        /// <param name="Field">Field to be initialized</param>
        /// <param name="MapProperties">List of WzImageProperty loaded from the Field's Image from Wz.</param>
        public static void SetTemplateField(TemplateField Template, ref List<WzImageProperty> MapProperties) {
            foreach (WzImageProperty Node in MapProperties) {
                if (Node == null) continue;
                
                switch (Node.Name) {
                    case "back": {}
                        break;
                    case "clock": {}
                        break;
                    case "foothold":
                        LoadFootholds(Template, Node);
                        break;
                    case "info":
                        LoadInfo(Template, Node);
                        break;
                    case "ladderRope": {}
                        break;
                    case "life":
                        if (Template.LoadLife) LoadLife(Template, Node);
                        break;
                    case "miniMap": 
                        break;
                    case "portal":
                        if (Template.LoadPortals) LoadPortals(Template, Node);
                        break;
                    case "reactor": {}
                        break;
                    default:
                        if (!int.TryParse(Node.Name, out int _)) {
                            Console.WriteLine($"Unhandled Map Node: {Node.Name, 10}({Node.GetType()})");   
                        }
                        break;
                }
            }
        }

        private static void PrintDirectory(WzImageProperty Parent) {
            foreach(WzImageProperty InternalProperty in Parent.WzProperties) {
                Console.WriteLine($"-- {InternalProperty.Name}({InternalProperty.GetType()})");
                Console.WriteLine($"---- {InternalProperty.WzValue}");
                if (InternalProperty.Name == "fieldLimit") {
                    foreach (FieldLimitType Type in Enum.GetValues(typeof(FieldLimitType))) {
                        Console.WriteLine($"{Type} : {Type.Check((int)InternalProperty.WzValue)}");
                    }
                }
            }
        }
        
        private static void LoadFootholds(TemplateField Template, WzImageProperty FootholdsImage) {
            Dictionary<uint, Foothold> Footholds = new Dictionary<uint, Foothold>();
            
            foreach (WzImageProperty Collection in FootholdsImage.WzProperties) {
                foreach (WzImageProperty Parent in Collection.WzProperties) {
                    foreach (WzImageProperty Child in Parent.WzProperties) {
                        if (!int.TryParse(Child.Name, out int ChildID)) continue;
                        Foothold Foothold = new Foothold();
                        
                        foreach (WzImageProperty Property in Child.WzProperties) {
                            switch (Property.Name) {
                                case "next":
                                    Foothold.Next = ((WzIntProperty) Property).Value;
                                    break;
                                case "prev": 
                                    Foothold.Prev = ((WzIntProperty) Property).Value;
                                    break;
                                case "x1":
                                    Foothold.X1 = ((WzIntProperty) Property).Value;
                                    break;
                                case "x2":
                                    Foothold.X2 = ((WzIntProperty) Property).Value;
                                    break;
                                case "y1":
                                    Foothold.Y1 = ((WzIntProperty) Property).Value;
                                    break;
                                case "y2":
                                    Foothold.Y2 = ((WzIntProperty) Property).Value;
                                    break;
                                case "piece":
                                    break;
                                default:
                                    Console.WriteLine($"Unhandled Field/Foothold Property: {Property.Name, 10}({Property.PropertyType})");
                                    break;
                            }
                        }

                        Foothold.ID = ChildID;
                        Foothold.SetEndPoints();
                        Footholds.Add((uint) ChildID, Foothold);
                    } 
                }
            }

            Template.Footholds = Footholds.Select(Entry => Entry.Value).ToArray();
        }

        /*
             id      =WzStringProperty
             type    =WzStringProperty
             mobTime =WzIntProperty
             f       =WzIntProperty
             hide    =WzIntProperty
             fh      =WzIntProperty
             cy      =WzIntProperty
             rx0     =WzIntProperty
             rx1     =WzIntProperty
             x       =WzIntProperty
             y       =WzIntProperty
         */
        private static void LoadLife(TemplateField Template, WzImageProperty LifeImage) {
            foreach (WzImageProperty Life in LifeImage.WzProperties) {
                if (!int.TryParse(Life.Name, out int ID)) continue;
                
                EntityType? Type = null;
                FieldLifeEntry FieldLife = new FieldLifeEntry();

                foreach (WzImageProperty Property in Life.WzProperties) {
                    switch (Property.Name) {
                        case "id":
                            if (int.TryParse(((WzStringProperty) Property).Value, out int LifeID)) {
                                FieldLife.ID = (uint) LifeID;
                            } 
                            break;
                        case "type":
                            string Value = ((WzStringProperty) Property).Value;
                            switch (Value) {
                                case "m":
                                    Type = EntityType.Mob;
                                    break;
                                case "n":
                                    Type = EntityType.Npc;
                                    break;
                                case "r":
                                    Type = EntityType.Reactor;
                                    break;
                                default:
                                    Console.WriteLine($"Unhandled Entity Type: {Value}");
                                    break;
                            }
                            break;
                        case "mobTime":
                            FieldLife.MobTime = ((WzIntProperty) Property).Value;
                            break;
                        case "f":
                            FieldLife.Flipped = ((WzIntProperty) Property).Value == 1;
                            break;
                        case "hide":
                            FieldLife.Hidden = ((WzIntProperty) Property).Value == 1;
                            break;
                        case "fh":
                            FieldLife.FootholdID = ((WzIntProperty) Property).Value;
                            break;
                        case "cy":
                            FieldLife.Cy = ((WzIntProperty) Property).Value;
                            break;
                        case "rx0":
                            FieldLife.Rx0 = ((WzIntProperty) Property).Value;
                            break;
                        case "rx1":
                            FieldLife.Rx1 = ((WzIntProperty) Property).Value;
                            break;
                        case "x":
                            FieldLife.X = ((WzIntProperty) Property).Value;
                            break;
                        case "y":
                            FieldLife.Y = ((WzIntProperty) Property).Value;
                            break;
                        default: 
                            Console.WriteLine($"Unhandled Field/Life Property: {Property.Name, 10}({Property.PropertyType})");
                            break;
                    }
                }

                if (Type.HasValue) {
                    if (Template.Life.TryGetValue(Type.Value, out Dictionary<uint, FieldLifeEntry> FieldLifeEntries)) {
                        FieldLifeEntries.Add((Type == EntityType.Mob ? (uint) FieldLife.FootholdID : FieldLife.ID), FieldLife);
                    } else {
                        Console.WriteLine($"Unable to add field life entry: id={ID}, type={Type}");
                    }
                }
            }
        }

        /*
            version      =WzIntProperty
            cloud        =WzIntProperty
            town         =WzIntProperty
            returnMap    =WzIntProperty
            forcedReturn =WzIntProperty
            mobRate      =WzFloatProperty
            bgm          =WzStringProperty
            mapMark      =WzStringProperty
            hideMinimap  =WzIntProperty
            fieldLimit   =WzIntProperty
            VRTop        =WzIntProperty
            VRLeft       =WzIntProperty
            VRBottom     =WzIntProperty
            VRRight      =WzIntProperty
            swim         =WzIntProperty
         */
        private static void LoadInfo(TemplateField Template, WzImageProperty InfoImage) {
            foreach (WzImageProperty Property in InfoImage.WzProperties) {
                switch (Property.Name) {
                    case "bgm":
                        Template.BackgroundMusic = ((WzStringProperty) Property).Value;
                        break;
                    case "fieldLimit":
                        Template.FieldLimits = new bool[Enum.GetNames(typeof(FieldLimitType)).Length];
                        foreach (FieldLimitType Type in Enum.GetValues(typeof(FieldLimitType))) {
                            Template.FieldLimits[(int) Type] = Type.Check((int) Property.WzValue);
                        }
                        break;
                    case "forcedReturn":
                        Template.ForcedReturn = ((WzIntProperty) Property).Value;
                        break;
                    case "returnMap":
                        break;
                    case "mobRate":
                        Template.MobRate = ((WzFloatProperty) Property).Value;
                        break;
                    case "onFirstUserEnter":
                        Template.OnFirstUserEnter = ((WzStringProperty) Property).Value;
                        break;
                    case "onUserEnter":
                        Template.OnUserEnter = ((WzStringProperty) Property).Value;
                        break;
                    case "fly":
                        Template.Fly = ((WzIntProperty) Property).Value == 1;
                        break;
                    case "swim":
                        Template.Swim = ((WzIntProperty) Property).Value == 1;
                        break;
                    case "town":
                        Template.Town = ((WzIntProperty) Property).Value == 1;
                        break;
                    case "cloud": 
                    case "hideMinimap": 
                    case "mapMark":
                    case "version":
                    case "VRBottom":
                    case "VRLeft":
                    case "VRRight":
                    case "VRTop":
                        break;
                    default:
                        Console.WriteLine($"Unhandled Map/Info Property: {Property.Name, 10}({Property.GetType()})");
                        break;
                }
            }
        }

        /*
            pn = WzStringProperty
            pt = WzIntProperty
            x =  WzIntProperty
            y =  WzIntProperty
            tm = WzIntProperty
            tn = WzStringProperty
         */
        private static void LoadPortals(TemplateField Template, WzImageProperty PortalImage) {
            List<Portal> Portals = new List<Portal>();
            foreach(WzImageProperty PortalNode in PortalImage.WzProperties) {
                Portal Portal = new Portal();
                foreach (WzImageProperty Property in PortalNode.WzProperties) {
                    switch (Property.Name) {
                        case "pn":
                            Portal.Name = ((WzStringProperty) Property).Value;
                            break;
                        case "pt":
                            Portal.TargetPortalID = ((WzIntProperty) Property).Value;
                            break; 
                        case "tm":
                            Portal.TargetMap = ((WzIntProperty) Property).Value;
                            break;
                        case "tn":
                            Portal.TargetPortalName = ((WzStringProperty) Property).Value;
                            break;
                        case "x":
                            Portal.X = ((WzIntProperty) Property).Value;
                            break;
                        case "y":
                            Portal.Y = ((WzIntProperty) Property).Value;
                            break;
                        default:
                            Console.WriteLine($"Unhandled Portal Property: {Property.Name}");
                            break;
                    }
                }
                Portals.Add(Portal);
            }

            Template.Portals = Portals.ToArray();
        }
    }
}