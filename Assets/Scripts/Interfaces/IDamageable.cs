using System;

namespace Interfaces
{
    public interface IDamageable
    {
        public event Action OnDeath;
        
        public void Damage(float damage);
    }
}