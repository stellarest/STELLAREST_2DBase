
namespace STELLAREST_2D
{
    public class CreatureController : BaseController
    {
        protected float _speed = 1f;
        public int Hp { get; set; } = 100;
        public int MaxHp { get; set; } = 100;

        public virtual void OnDamaged(BaseController attacker, int damage)
        {
            Hp -= damage;
            if (Hp <= 0)
            {
                Hp = 0;
                OnDead();
            }
        }

        protected virtual void OnDead()
        {
        }
    }
}