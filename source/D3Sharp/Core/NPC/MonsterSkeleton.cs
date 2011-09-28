using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using D3Sharp.Net.Game;

namespace D3Sharp.Core.NPC
{
    class MonsterSkeleton : Monster
    {
        public MonsterSkeleton()
            : base(11488, 11465, 11529, 54729, 11468) 
        {
            npcType = NPCList.Zombie_A;
            HP = 100;
        }
        

        //
        // Tick
        //
        public override void Tick()
        {
            if (Game == null || Game.position == null)
                return;

            if (HP <= 0)
            {
                Die();
                return;
            }

            float distFromPlayer = Game.position.Distance(Position);

            base.Tick();

            

            if (distFromPlayer > 1 && distFromPlayer < 64)
            {
              //  LookToPlayer();
                if (!ismoving)
                {
                    ismoving = true;
                    MoveToPlayer();
                }
            }
            else if (distFromPlayer < 4)
            {
               
              //  MeleeAttack(6);
            }
            else
            {
                nextThink += 5;
            }
        }
    }
}
