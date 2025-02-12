using Brains;
using UnityEngine;

namespace Data.Structs
{
    public struct DefenseArgs
    {
        public readonly DefenseBrain Brain;

        public DefenseArgs(DefenseBrain brain)
        {
            Brain = brain;
        }
    }
}