using Placement;

namespace Data.Structs
{
    public struct EnemyArgs
    {
        public readonly GridManager GridManager;
        public readonly EnemyBrain EnemyBrain;

        public EnemyArgs(GridManager gridManager, EnemyBrain enemyBrain)
        {
            GridManager = gridManager;
            EnemyBrain = enemyBrain;
        }
    }
}