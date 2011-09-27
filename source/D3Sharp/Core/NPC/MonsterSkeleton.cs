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
            : base(0x26db, 0x26dd, 0x26dc, 0x26dc, 0)
        {
            npcType = NPCList.Skeleton_A;
            HP = 100;
        }

        //
        // Tick
        //
        public override void Tick()
        {
            if (Game == null || Game.position == null)
                return;

            float distFromPlayer = Game.position.Distance(Position);

            base.Tick();

            if (distFromPlayer < 10)
            {
                MeleeAttack(6);
            }
            //else if (distFromPlayer < 64)
           //{
                //MoveToPlayer();
          //  }
            else
            {
                nextThink += 5;
            }
        }
    }
}
