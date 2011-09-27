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
<<<<<<< HEAD
        
=======
        private Vector3D position = new Vector3D();
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
        protected int spawnId;
        protected NPCList npcType;
        private bool isDead = false;
        private Random rand = new Random();

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

<<<<<<< HEAD
        public Monster(short Pain, short Attack, short walk, short idle, short Death)
=======
        public Monster(int Pain, int Attack, int walk, int idle, int Death)
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
        {
            PainAnimation = Pain;
            AttackAnimation = Attack;
            DeathAnimation = Death;
            WalkAnimation = walk;
            IdleAnimation = idle;

        }

<<<<<<< HEAD
        public override void Collide(BasicNPC actor)
        {
            nextThink += 100;
        }

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
        private void MoveActor()
        {
            Game.SendMessage(new ACDTranslateNormalMessage()
            {
                Field0 = ID,
                Field1 = Position,
                Id = 0x006E,
<<<<<<< HEAD
                Field2 = (float)Math.Atan2(Position.Field0 - Game.position.Field0, Position.Field1 - Game.position.Field1),
=======
                Field2 = 0.08464038f,
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
                Field3 = false,
                Field4 = 1.0f,
                 Field5 = 0,
                Field6 = 69728,
            });


<<<<<<< HEAD
            Game.tick += 2;
            Game.SendMessage(new EndOfTickMessage()
            {
                Id = 0x008D,
                Field0 = Game.tick - 2,
                Field1 = Game.tick
            });

            Game.FlushOutgoingBuffer();
=======
            Game.tick += 20;
            Game.SendMessage(new EndOfTickMessage()
            {
                Id = 0x008D,
                Field0 = Game.tick - 20,
                Field1 = Game.tick
            });
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
        }

        public void MoveToPlayer()
        {
<<<<<<< HEAD
            if (Game == null || Game.position == null)
                return;

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            state = MonsterState.MOB_STATE_WALKING_TO_PLAYER;

            xvel = Game.position.Field0 - Position.Field0;
            yvel = Game.position.Field1 - Position.Field1;

<<<<<<< HEAD
            Position.Field0 += xvel * 0.108f;
            Position.Field1 += yvel * 0.108f;

            PlayAnimation(WalkAnimation);

            nextThink += 3;
=======
            Position.Field0 += xvel * 0.0608f;
            Position.Field1 += yvel * 0.0608f;


            PlayAnimation(WalkAnimation);
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299

            MoveActor();
        }

        protected void MeleeAttack(int damage)
        {
<<<<<<< HEAD
=======

            if (state == MonsterState.MOB_STATE_PAIN)
            {
                return;
            }

            if (state != MonsterState.MOB_STATE_ATTACKING)
            {
                state = MonsterState.MOB_STATE_ATTACKING;
                nextThink += 60;
                animTime = 3;
            }
            
            if (animTime < 0)
            {
                state = MonsterState.MOB_STATE_IDLE;
                PlayAnimation(WalkAnimation);
                nextThink += 450;
                return;
            }

>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            PlayAnimation(AttackAnimation);

            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });

<<<<<<< HEAD
            nextThink += 40;

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
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

<<<<<<< HEAD
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
                    
=======
            PlayAnimation(PainAnimation);
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299

            

            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });
<<<<<<< HEAD

         

            Game.FlushOutgoingBuffer();
=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
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
<<<<<<< HEAD

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

=======
            var killAni = new int[]{
                    0x2cd7,
                    0x2cd4,
                    0x01b378,
                    0x2cdc,
                    0x02f2,
                    0x2ccf,
                    0x2cd0,
                    0x2cd1,
                    0x2cd2,
                    0x2cd3,
                    0x2cd5,
                    0x01b144,
                    0x2cd6,
                    0x2cd8,
                    0x2cda,
                    0x2cd9
            };
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
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

<<<<<<< HEAD
            int ani = DeathAnimation;
=======
            int ani = killAni[rand.Next(killAni.Length)];
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299

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
<<<<<<< HEAD

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            Game.packetId += 10 * 2;
            Game.SendMessage(new DWordDataMessage()
            {
                Id = 0x89,
                Field0 = Game.packetId,
            });
<<<<<<< HEAD
           

            Game.FlushOutgoingBuffer();
=======

>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
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
