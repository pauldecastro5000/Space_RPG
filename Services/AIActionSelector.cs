using Space_RPG.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Space_RPG.Services
{
    public static class AIActionSelector
    {
        private static readonly Random _random = new Random();

        public static CrewAction GetAction(Crew status)
        {
            Dictionary<CrewAction, float> actionWeights = new Dictionary<CrewAction, float>();

            // Lower hunger means more hungry
            float eatDesire = 100f - status.Hunger;

            // Higher fatigue means more sleepy
            float sleepDesire = status.Fatigue;

            // Work desire decreases when tired or hungry
            float workDesire =
                ((status.Hunger * 0.5f) + ((100f - status.Fatigue) * 0.5f));

            actionWeights[CrewAction.Eat] = Math.Max(1f, eatDesire);
            actionWeights[CrewAction.Sleep] = Math.Max(1f, sleepDesire);
            actionWeights[CrewAction.Work] = Math.Max(1f, workDesire);

            return GetWeightedRandomAction(actionWeights);
        }

        private static CrewAction GetWeightedRandomAction(
            Dictionary<CrewAction, float> weights)
        {
            float totalWeight = weights.Values.Sum();

            float randomValue = (float)(_random.NextDouble() * totalWeight);

            float current = 0;

            foreach (var item in weights)
            {
                current += item.Value;

                if (randomValue <= current)
                {
                    return item.Key;
                }
            }

            return weights.Keys.First();
        }
    }
}
