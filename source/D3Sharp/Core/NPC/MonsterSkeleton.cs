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
            : base(9811, 9792, 9811, 9831, 9807)
        {
            npcType = NPCList.SkeletonArcher_A;
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

            //LookAtPlayer();

            if (distFromPlayer > 13 && distFromPlayer < 64 )
            {
              //  MeleeAttack(6);
            }
            else if (distFromPlayer < 13)
            {

            }
            else
            {
                nextThink += 5;
            }
        }
    }
}
