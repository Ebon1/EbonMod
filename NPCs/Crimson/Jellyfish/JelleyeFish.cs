using EbonianMod.Projectiles.VFXProjectiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.ItemDropRules;

namespace EbonianMod.NPCs.Crimson.Jellyfish
{
    public class JelleyeFish : ModNPC
    {
        public override void SetDefaults()
        {
            NPC.aiStyle = -1;
            NPC.lifeMax = 1000;
            NPC.damage = 0;
            NPC.noTileCollide = true;
            NPC.defense = 20;
            NPC.knockBackResist = 0;
            NPC.width = 82;
            NPC.height = 78;
            NPC.lavaImmune = true;
            NPC.noGravity = true;
            NPC.HitSound = SoundID.NPCHit1;
            NPC.netAlways = true;
            NPC.value = Item.buyPrice(0, 20);
        }
        Verlet[] eyeVerlets = new Verlet[4];
        Verlet[] tentVerlets = new Verlet[4];
        public override void OnSpawn(IEntitySource source)
        {
            for (int i = 0; i < eyeVerlets.Length; i++)
            {
                eyeVerlets[i] = new Verlet(NPC.Center + new Vector2((i - 1) * 3, 0), 6, 2 + i * 6 + Main.rand.Next(5, 12), 5, lastPointLocked: false, stiffness: 30);
            }
            for (int i = 0; i < tentVerlets.Length; i++)
            {
                tentVerlets[i] = new Verlet(NPC.Center + new Vector2((i - 3) * 4, 0), 6, 10 + i * 3, 7, lastPointLocked: false, stiffness: 15);
            }
        }
        public float AIState
        {
            get => NPC.ai[0];
            set => NPC.ai[0] = value;
        }
        public float AITimer
        {
            get => NPC.ai[1];
            set => NPC.ai[1] = value;
        }
        public float AITimer2
        {
            get => NPC.ai[2];
            set => NPC.ai[2] = value;
        }
        Vector2 scaleMult = new Vector2(1, 1);
        Vector2 _vel;
        public override void AI()
        {
            NPC.TargetClosest(false);
            Player player = Main.player[NPC.target];
            AITimer++;
            AITimer2++;
            if (AITimer2 > 40 && AITimer2 < 100)
            {
                NPC.velocity = Vector2.Lerp(NPC.velocity, Vector2.Clamp(NPC.Center.FromAToB(player.Center, true, true), new Vector2(-1, -1), new Vector2(1, -0.01f)) * 0.1f, 0.05f);
                NPC.rotation = Helper.LerpAngle(NPC.rotation, Vector2.Clamp(NPC.Center.FromAToB(player.Center), new Vector2(-1, -1), new Vector2(1, -0.01f)).ToRotation() + PiOver2, 0.1f);
            }
            if (AITimer2 >= 100)
            {
                Projectile.NewProjectile(null, NPC.Center, -NPC.velocity.RotatedByRandom(PiOver2) * 0.5f, ModContent.ProjectileType<HostileGibs>(), 20, 0);
                if (AITimer2 == 100)
                    _vel = Vector2.Clamp(NPC.Center.FromAToB(player.Center - new Vector2(0, 100)) * 10, new Vector2(-1, -1), new Vector2(1, 0.01f));
                Vector2 oldVel = _vel;
                if (_vel.X.CloseTo(0, 0.05f)) _vel.X = 1 * (oldVel.X > 0 ? 1 : -1);
                NPC.velocity = Vector2.Lerp(NPC.velocity, _vel * 10, 0.1f);
                NPC.rotation = Helper.LerpAngle(NPC.rotation, NPC.velocity.ToRotation() + PiOver2, 0.1f);
                scaleMult = new Vector2(0.8f, 1.2f);

            }
            if (AITimer2 > 110) AITimer2 = Main.rand.Next(-50, 80);
            NPC.velocity *= 0.999f;
            scaleMult = Vector2.Lerp(scaleMult, Vector2.One, 0.1f);

            for (int i = 0; i < eyeVerlets.Length; i++)
                if (eyeVerlets[i] != null)
                {
                    eyeVerlets[i].gravity = Lerp(eyeVerlets[i].gravity, MathF.Sin(i + AITimer * 0.025f) * 2 + 4, 0.1f);
                    if (AITimer % 2 == 0)
                    {
                        Vector2 dir = -NPC.rotation.ToRotationVector2().RotatedBy(-PiOver2).RotatedBy((i - eyeVerlets.Length / 2f) * 0.5f * MathF.Sin(i + AITimer * 0.1f));
                        if (dir != Vector2.Zero)
                            dir.Normalize();
                        else dir = Vector2.UnitY;
                        eyeVerlets[i].gravityDirection = dir;
                    }
                    eyeVerlets[i].Update(NPC.Center + new Vector2(MathF.Abs(i - 1) * 10, -4).RotatedBy(NPC.rotation - PiOver2), eyeVerlets[i].lastP.position);
                }
            for (int i = 0; i < tentVerlets.Length; i++)
                if (tentVerlets[i] != null)
                {
                    tentVerlets[i].gravity = Lerp(tentVerlets[i].gravity, MathF.Sin(i + AITimer * 0.05f) + 3, 0.1f);
                    if (AITimer % 2 == 0)
                    {
                        Vector2 dir = -NPC.rotation.ToRotationVector2().RotatedBy(-PiOver2).RotatedBy((i - tentVerlets.Length / 2f) * 0.5f * MathF.Sin(i + AITimer * 0.1f));
                        if (dir != Vector2.Zero)
                            dir.Normalize();
                        else dir = Vector2.UnitY;
                        tentVerlets[i].gravityDirection = dir;
                    }
                    tentVerlets[i].Update(NPC.Center + new Vector2((i - 2) * 8, -4).RotatedBy(NPC.rotation - PiOver2), tentVerlets[i].lastP.position);
                }

        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            for (int i = 0; i < tentVerlets.Length; i++)
                if (tentVerlets[i] != null)
                    tentVerlets[i].Draw(spriteBatch, new VerletDrawData(Texture + "_Chain", null, Texture + "_Tip"));

            for (int i = 0; i < eyeVerlets.Length; i++)
                if (eyeVerlets[i] != null)
                    eyeVerlets[i].Draw(spriteBatch, new VerletDrawData(Texture + "_Chain", null, Texture + "_Eye"));

            Texture2D tex = Helper.GetTexture(Texture);
            spriteBatch.Draw(tex, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.Size / 2, scaleMult * NPC.scale, SpriteEffects.None, 0);
            return false;
        }
    }
}
