﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using HUD;
using RainMeadow;
using RWCustom;
using UnityEngine;
using RainMeadow;
namespace RainMeadow
{
    public partial class OnlinePlayerSpecificHud
    {
        public partial class JollyPlayerArrow : JollyPointer
        {
            public bool pointing;

            public float blink;

            public int fadeAwayCounter;

            public bool hide;

            public FLabel label;

            public int lastRoom = -1;

            public string playerName;

            public int shortcutWaitTime;

            public int initialWaitTime;

            public Color mainColor;

            public Color inverColor;

            public int frequency;

            public bool friendsViewToggle = false;

            public StoryAvatarSettings personaSettings;
            public int maxAttempts = 1;

            public List<string> playersWithArrows;

            public JollyPlayerArrow(OnlinePlayerSpecificHud jollyHud, string name)
                : base(jollyHud)
            {
                this.personaSettings = (StoryAvatarSettings)OnlineManager.lobby.gameMode.avatarSettings;

                RainMeadow.Debug("Adding Player pointer to " + base.PlayerState.playerNumber);
                bodyPos = new Vector2(0f, 0f);
                lastBodyPos = bodyPos;
                blink = 1f;
                playerName = name;
                mainColor = personaSettings.bodyColor;
                inverColor = Color.white; //TODO
                gradient = new FSprite("Futile_White")
                {
                    shader = base.jollyHud.hud.rainWorld.Shaders["FlatLight"],
                    alpha = 0f,
                    x = -1000f,
                    color = inverColor
                };
                jollyHud.fContainer.AddChild(gradient);
                mainSprite = new FSprite("Multiplayer_Arrow");
                jollyHud.fContainer.AddChild(mainSprite);
                label = new FLabel(Custom.GetFont(), playerName);
                jollyHud.fContainer.AddChild(label);
                initialWaitTime = Player.InitialShortcutWaitTime;
            }

            // TODO: Every character should get an arrow

            public override void Update()
            {
                base.Update();
                blink = Mathf.Max(0f, blink - 0.0125f);
                alpha = Custom.LerpAndTick(alpha, Mathf.InverseLerp(80f, 20f, fadeAwayCounter), 0.08f, 71f / (678f * (float)Math.PI));
                mainColor = jollyHud.playerColor;


                /*            var players = OnlineManager.lobby.playerAvatars
                                    .Where(avatar => avatar.type != (byte)OnlineEntity.EntityId.IdType.none)
                                    .Select(avatar => avatar.FindEntity(true))
                                    .OfType<OnlinePhysicalObject>()
                                    .Select(opo => opo.apo as AbstractCreature)
                                    .Zip(OnlineManager.players, (creature, player) => new { AbstractCreature = creature, PlayerName = player })
                                    .ToList();

                            if (playersWithArrows.Count != players.Count)
                            {

                                RainMeadow.Debug("Adding extra HUD to players who just joined");
                                for (int i = 0; i < players.Count; i++)
                                {

                                    if (maxAttempts <= 0)
                                    {
                                        break; // Break out of the loop if maxAttempts is reached
                                    }
                                    // Right now I'm giving up my arrow over to Player 2, need to fix that.
                                    // Infinte loop due to my list
                                    if (!playersWithArrows.Contains(players[i].PlayerName.id.name))
                                    {
                                        OnlinePlayerSpecificHud otherPlayerJollyHud = new OnlinePlayerSpecificHud(this.jollyHud.hud, this.jollyHud.fContainer, players[i].AbstractCreature, players[i].PlayerName.id.name, Color.white);
                                        this.jollyHud.hud.parts.Add(otherPlayerJollyHud);
                                        playersWithArrows.Add(players[i].PlayerName.id.name);
                                        maxAttempts--;

                                    }
                                }
                            }*/


                if (jollyHud.Camera.room == null)
                {
                    hide = true;
                }

                lastRoom = jollyHud.Camera.cameraNumber;
                if (base.PlayerState.permaDead)
                {
                    slatedForDeletion = true;
                    hide = true;
                }

                if (playerName == string.Empty)
                {
                    playerName = "";

                    label.text = playerName;
                    size.x = 5 * playerName.Length;
                }


                if (!jollyHud.PlayerRoomBeingViewed || forceHide || !knownPos)
                {
                    hide = true;
                }
                else
                {
                    hide = false;
                }

                if (hide)
                {
                    alpha = 0f;
                    lastAlpha = 0f;
                    mainColor = jollyHud.playerColor;
                }

                pointing = false;
                if (jollyHud.RealizedPlayer == null)
                {
                    return;
                }

                PhysicalObject objectPointed = jollyHud.RealizedPlayer.objectPointed;
                if (objectPointed != null && objectPointed.jollyBeingPointedCounter > 35)
                {
                    blink = 1f;
                    fadeAwayCounter = 0;
                    timer = 0;
                    pointing = true;
                }

                if (pointing && timer < 20)
                {
                    blink = 1f;
                    fadeAwayCounter = 0;
                    timer = 0;
                }

                if (Input.GetKey(RainMeadow.rainMeadowOptions.FriendsListKey.Value) || nearEdge || jollyHud.RealizedPlayer.inShortcut)
                {
                    friendsViewToggle = true;

                }

                else
                {
                    friendsViewToggle = false;
                }

                if (friendsViewToggle)
                {
                    blink = 1f;
                    fadeAwayCounter = 0;
                    timer = 0;
                }
                else
                {
                    fadeAwayCounter++;


                }
                /*

                            if ((jollyHud.RealizedPlayer.RevealMap || jollyHud.RealizedPlayer.showKarmaFoodRainTime > 0 || nearEdge || jollyHud.RealizedPlayer.inShortcut) && timer > 20)
                                {
                                    // Keeps the arrow around
                                    blink = 1f;
                                    fadeAwayCounter = 0;
                                    timer = 0;
                                }

                                if (timer > 10 && !Custom.DistLess(jollyHud.RealizedPlayer.firstChunk.lastPos, jollyHud.RealizedPlayer.firstChunk.pos, 3f))
                                {
                                    fadeAwayCounter++;
                                }

                                if (fadeAwayCounter > 0)
                                {
                                    fadeAwayCounter++;
                                }
                */


                timer++;
                frequency++;
                frequency %= 40;
                if (timer > 100)
                {
                    timer = 100;
                }

                if (fadeAwayCounter > 100)
                {
                    fadeAwayCounter = 100;
                }

                try
                {
                    ShortcutHandler.ShortCutVessel shortCutVessel = jollyHud.Camera.game.shortcuts?.transportVessels?.FirstOrDefault((ShortcutHandler.ShortCutVessel x) => x.creature == jollyHud.RealizedPlayer);
                    if (shortCutVessel != null)
                    {
                        shortcutWaitTime = shortCutVessel.wait;
                    }
                }
                catch (Exception ex)
                {
                    RainMeadow.Debug(ex.ToString());
                }

                hidden = Mathf.Abs(alpha) - 0.05f < 0f;
            }

            public Vector2 ClampScreenEdge(Vector2 input)
            {
                input.x = Mathf.Clamp(input.x, screenEdge, jollyHud.hud.rainWorld.options.ScreenSize.x - (float)screenEdge);
                input.y = Mathf.Clamp(input.y, screenEdge, jollyHud.hud.rainWorld.options.ScreenSize.y - (float)screenEdge);
                return input;
            }

            public override void Draw(float timeStacker)
            {
                base.Draw(timeStacker);

                Vector2 input = Vector2.Lerp(lastBodyPos, bodyPos, timeStacker) + new Vector2(0.01f, 40f);
                Vector2 input2 = Vector2.Lerp(lastTargetPos, targetPos, timeStacker) + new Vector2(0.01f, 40f);
                input = ClampScreenEdge(input);
                input2 = ClampScreenEdge(input2);
                float rotation = 0f;
                if (Custom.Dist(bodyPos, input) > 20f)
                {
                    rotation = Custom.AimFromOneVectorToAnother(bodyPos, input);
                    rotation = Mathf.Round(rotation / 90f) * 90f;
                }

                if (mainSprite != null)
                {
                    mainSprite.x = input.x;
                    mainSprite.y = input.y;
                    mainSprite.rotation = rotation;
                }

                if (label != null)
                {
                    label.x = input.x;
                    label.y = input2.y + 20f;
                }

                float num = Mathf.Pow(Mathf.Max(0f, Mathf.Lerp(lastAlpha, alpha, timeStacker)), 0.7f);
                if (shortcutWaitTime > 0 && !hide)
                {
                    num = 1f;
                    alpha = 1f;
                    float num2 = (float)frequency / 40f;
                    float t = (float)shortcutWaitTime / (float)initialWaitTime;
                    float num3 = Mathf.Max(0f, Mathf.Pow(Mathf.Lerp(17f, 4f, t), 1.2f));
                    float t2 = 0.5f * (0.8f + Mathf.Sin(num3 * num2));
                    float t3 = Mathf.Lerp(0.01f, 0.6f, t2);
                    mainColor = Color.Lerp(jollyHud.playerColor, Color.white, t3); // TODO
                }

                gradient.x = input.x;
                gradient.y = input.y + 10f;
                gradient.y = input.y;
                gradient.scale = Mathf.Lerp(80f, 110f, num) / 16f;
                gradient.alpha = 0.17f * Mathf.Pow(num, 2f);
                label.color = mainColor;
                mainSprite.color = mainColor;
                label.alpha = (pointing ? 0f : num);
                mainSprite.alpha = num;
            }

            public override void ClearSprites()
            {
                base.ClearSprites();
                label.RemoveFromContainer();
                mainSprite.RemoveFromContainer();
                gradient.RemoveFromContainer();
            }
        }
    }
}
