using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using D3Sharp.Net.Game;
using D3Sharp.Net.Game.Message.Fields;
using D3Sharp.Net.Game.Message.Definitions.ACD;
using D3Sharp.Net.Game.Message.Definitions.Misc;
using D3Sharp.Net.Game.Message.Definitions.Animation;
using D3Sharp.Net.Game.Message.Definitions.Effect;
using D3Sharp.Net.Game.Message.Definitions.Attribute;
using D3Sharp.Net.Game.Message.Definitions.Game;
using D3Sharp.Net.Game.Messages;

namespace D3Sharp.Core.NPC
{
    

    //
    // Monster
    //
    public abstract class Monster : BasicNPC
    {
        
        protected int spawnId;
        protected NPCList npcType;
        private bool isDead = false;

        public readonly int experience = 90;

        float xvel = 0;
        float yvel = 0;


        public enum MonsterState
        {
            MOB_STATE_IDLE = 0,
            MOB_STATE_ATTACKING,
            MOB_STATE_WALKING_TO_PLAYER,
            MOB_STATE_PAIN,
            MOB_STATE_DIEING,
            MOB_STATE_DEAD
        };

        protected MonsterState state = MonsterState.MOB_STATE_IDLE;

        protected readonly int PainAnimation;
        protected readonly int AttackAnimation;
        protected readonly int DeathAnimation;
        protected readonly int WalkAnimation;
        protected readonly int IdleAnimation;

        private int animTime = 0;

        public Monster(short Pain, short Attack, short walk, short idle, short Death)
        {
            PainAnimation = Pain;
            AttackAnimation = Attack;
            DeathAnimation = Death;
            WalkAnimation = walk;
            IdleAnimation = idle;

        }

        public override void Collide(BasicNPC actor)
        {
            nextThink += 100;
        }

        protected void LookAtPlayer()
        {
            Game.SendMessage(new ACDTranslateNormalMessage()
            {
                Field0 = ID,
                Field1 = Position,
                Id = 0x006E,
                Field2 = (float)Math.Atan2(Position.Field0 - Game.position.Field0, Position.Field1 - Game.position.Field1),
                Field3 = false,
                Field4 = 1.0f,
                Field5 = 0,
                Field6 = 69728,
            });


            Game.tick += 2;
            Game.SendMessage(new EndOfTickMessage()
            {
                Id = 0x008D,
                Field0 = Game.tick - 2,
                Field1 = Game.tick
            });

            Game.FlushOutgoingBuffer();
        }

        private void MoveActor()
        {
            Game.SendMessage(new ACDTranslateNormalMessage()
            {
                Field0 = ID,
                Field1 = Position,
                Id = 0x006E,
                Field2 = (float)Math.Atan2(Position.Field0 - Game.position.Field0, Position.Field1 - Game.position.Field1),
                Field3 = false,
                Field4 = 1.0f,
                 Field5 = 0,
                Field6 = 69728,
            });


            Game.tick += 2;
            Game.SendMessage(new EndOfTickMessage()
            {
                Id = 0x008D,
                Field0 = Game.tick - 2,
                Field1 = Game.tick
            });

            Game.FlushOutgoingBuffer();
        }

        public void MoveToPlayer()
        {
            if (Game == null || Game.position == null)
                return;

            state = MonsterState.MOB_STATE_WALKING_TO_PLAYER;

            xvel = Game.position.Field0 - Position.Field0;
            yvel = Game.position.Field1 - Position.Field1;

            Position.Field0 += xvel * 0.108f;
            Position.Field1 += yvel * 0.108f;

            PlayAnimation(WalkAnimation);

            nextThink += 3;

            MoveActor();
        }

        protected void MeleeAttack(int damage)
        {
            PlayAnimation(AttackAnimation);

            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });

            nextThink += 40;

            Game.FlushOutgoingBuffer();
        }

        public int SpawnID
        {
            get
            {
                return (int)npcType;
            }
        }

        private void PlayAnimation(int ani)
        {
            Game.SendMessage(new ANNDataMessage()
            {
                Id = 0x6d,
                Field0 = ID,
            });

            Game.SendMessage(new PlayAnimationMessage()
            {
                Id = 0x6c,
                Field0 = ID,
                Field1 = 0xb,
                Field2 = 0,
                tAnim = new PlayAnimationMessageSpec[1]
                {
                    new PlayAnimationMessageSpec()
                    {
                        Field0 = 0x2,
                        Field1 = ani,
                        Field2 = 0x0,
                        Field3 = 1f
                    }
                }
            });
        }

        public override void Tick()
        {
            if (animTime < 0)
            {
                state = MonsterState.MOB_STATE_IDLE;
            }
            else
            {
                animTime--;
            }
        }

        public void Pain(int damage)
        {
            // Pain stops everything.
            state = MonsterState.MOB_STATE_PAIN;
            nextThink +=50;
            HP -= damage;

            Game.SendMessage(new PlayEffectMessage()
            {
                Id = 0x7a,
                Field0 = ID,
                Field1 = 0x0,
                Field2 = 0x2,
            });
            Game.SendMessage(new PlayEffectMessage()
            {
                Id = 0x7a,
                Field0 = ID,
                Field1 = 0xc,
            });
            Game.SendMessage(new PlayHitEffectMessage()
            {
                Id = 0x7b,
                Field0 = ID,
                Field1 = 0x789E00E2,
                Field2 = 0x2,
                Field3 = false,
            });

            Game.SendMessage(new FloatingNumberMessage()
            {
                Id = 0xd0,
                Field0 = ID,
                Field1 = damage,
                Field2 = 0,
            });

            if (HP > 0)
            {
                Game.SendMessage(new AttributeSetValueMessage
                {
                    Id = 0x4c,
                    Field0 = ID,
                    Field1 = new NetAttributeKeyValue
                    {
                        Attribute = GameAttribute.Attributes[77],
                        Float = HP
                    }
                });

                PlayAnimation(PainAnimation);
            }
                    

            

            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });

         

            Game.FlushOutgoingBuffer();
        }

        public bool IsDead()
        {
            return HP <= 0;
        }

        public void Die()
        {
            if (isDead)
                return;

            isDead = true;

            Game.UpdateExperience(experience);

            Game.SendMessage(new AttributeSetValueMessage
            {
                Id = 0x4c,
                Field0 = ID,
                Field1 = new NetAttributeKeyValue
                {
                    Attribute = GameAttribute.Attributes[77],
                    Float = 0
                }
            });

            Game.SendMessage(new PlayEffectMessage()
            {
                Id = 0x7a,
                Field0 = ID,
                Field1 = 0x0,
                Field2 = 0x2,
            });
            Game.SendMessage(new PlayEffectMessage()
            {
                Id = 0x7a,
                Field0 = ID,
                Field1 = 0xc,
            });
            Game.SendMessage(new PlayHitEffectMessage()
            {
                Id = 0x7b,
                Field0 = ID,
                Field1 = 0x789E00E2,
                Field2 = 0x2,
                Field3 = false,
            });

            Game.SendMessage(new ANNDataMessage()
            {
                Id = 0x6d,
                Field0 = ID,
            });

            int ani = DeathAnimation;

            Game.SendMessage(new PlayAnimationMessage()
            {
                Id = 0x6c,
                Field0 = ID,
                Field1 = 0xb,
                Field2 = 0,
                tAnim = new PlayAnimationMessageSpec[1]
                {
                    new PlayAnimationMessageSpec()
                    {
                        Field0 = 0x2,
                        Field1 = ani,
                        Field2 = 0x0,
                        Field3 = 1f
                    }
                }
            });

            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });

            Game.SendMessage(new ANNDataMessage()
            {
                Id = 0xc5,
                Field0 = ID,
            });

            Game.SendMessage(new AttributeSetValueMessage
            {
                Id = 0x4c,
                Field0 = 0x789E00E2,
                Field1 = new NetAttributeKeyValue
                {
                    Attribute = GameAttribute.Attributes[33],
                    Float = 23
                }
            });

            Game.SendMessage(new AttributeSetValueMessage
            {
                Id = 0x4c,
                Field0 = ID,
                Field1 = new NetAttributeKeyValue
                {
                    Attribute = GameAttribute.Attributes[0x4d],
                    Float = 0
                }
            });

            Game.SendMessage(new AttributeSetValueMessage
            {
                Id = 0x4c,
                Field0 = ID,
                Field1 = new NetAttributeKeyValue
                {
                    Attribute = GameAttribute.Attributes[0x1c2],
                    Int = 1
                }
            });

            Game.SendMessage(new AttributeSetValueMessage
            {
                Id = 0x4c,
                Field0 = ID,
                Field1 = new NetAttributeKeyValue
                {
                    Attribute = GameAttribute.Attributes[0x1c5],
                    Int = 1
                }
            });
            Game.SendMessage(new PlayEffectMessage()
            {
                Id = 0x7a,
                Field0 = ID,
                Field1 = 0xc,
            });
            Game.SendMessage(new PlayEffectMessage()
            {
                Id = 0x7a,
                Field0 = ID,
                Field1 = 0x37,
            });
            Game.SendMessage(new PlayHitEffectMessage()
            {
                Id = 0x7b,
                Field0 = ID,
                Field1 = 0x789E00E2,
                Field2 = 0x2,
                Field3 = false,
            });

            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });
           

            Game.FlushOutgoingBuffer();
        }


        //
        // SetPosition
        //
        public void SetPosition(float x, float y, float z)
        {
            position.Field0 = x;
            position.Field1 = y;
            position.Field2 = z;
        }

        public Vector3D Position
        {
            get
            {
                return position;
            }
        }
    }
}
