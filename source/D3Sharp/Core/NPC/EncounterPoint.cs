using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using D3Sharp.Core.NPC;
using D3Sharp.Net.Game;
using D3Sharp.Core.Map;

using D3Sharp.Net.Game.Message.Fields;

namespace D3Sharp.Core.NPC
{
    public class EncounterPoint
    {
        IList<Monster> ObjectIdsSpawned;
        GameClient _client;
        Random rand = new Random();

        public static int SmallEncounter
        {
            get
            {
                return 3;
            }
        }

        public static int MedEncounter
        {
            get
            {
                return 5;
            }
        }

        //
        // GameEncounterPoint
        //
        public EncounterPoint(World world, GameClient client, Type monsterType, int numMonsters, Vector3D position)
        {
            ObjectIdsSpawned = new List<Monster>();
            _client = client;

            numMonsters += rand.Next(3);

            for (int i = 0; i < numMonsters; i++)
            {
                // Allocate the monster with the world.
                Monster monster = (Monster)world.SpawnNPC(monsterType);

                // Random spawn offset.
                float xpos = (float)(rand.NextDouble() * 20);
                float ypos = (float)(rand.NextDouble() * 20);
                
                // Init the monster and set the position with the randomized coordinates.
                monster.Init(_client.GetNextValidObjectID(), ref client);
                monster.SetPosition(position.Field0 + xpos, position.Field1 + ypos, position.Field2);

                // Spawn the monster.
                world.SpawnMob(client, monster.Position, monster);

                System.Threading.Thread.Sleep(15); // Required to not generate the same random value twice...

                ObjectIdsSpawned.Add(monster);
            }
        }

        //
        // GetMonsterByID
        //
        public Monster GetMonsterByID(int objId)
        {
            for (int i = 0; i < ObjectIdsSpawned.Count; i++)
            {
                if (ObjectIdsSpawned[i].ID == objId)
                    return ObjectIdsSpawned[i];
            }

            return null;
        }

        public void RemoveMonster(Monster monster)
        {
            ObjectIdsSpawned.Remove(monster);
        }

        
    }
}
