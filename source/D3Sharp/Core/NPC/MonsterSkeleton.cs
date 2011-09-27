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
<<<<<<< HEAD
            : base(9811, 9792, 9811, 9831, 9807)
        {
            npcType = NPCList.SkeletonArcher_A;
=======
            : base(0x26db, 0x26dd, 0x26dc, 0x26dc, 0)
        {
            npcType = NPCList.Skeleton_A;
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            HP = 100;
        }

        //
        // Tick
        //
        public override void Tick()
        {
            if (Game == null || Game.position == null)
                return;

<<<<<<< HEAD
            if (HP <= 0)
            {
                Die();
                return;
            }

=======
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            float distFromPlayer = Game.position.Distance(Position);

            base.Tick();

<<<<<<< HEAD
            if (distFromPlayer < 13)
            {
              //  MeleeAttack(6);
            }
           
=======
            if (distFromPlayer < 10)
            {
                MeleeAttack(6);
            }
            //else if (distFromPlayer < 64)
           //{
                //MoveToPlayer();
          //  }
>>>>>>> 733b6deda75b65d6f99c389afce86fd4f81be299
            else
            {
                nextThink += 5;
            }
        }
    }
}
