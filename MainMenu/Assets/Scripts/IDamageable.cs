/// <summary>
/// 데미지, 힐 관리 인터페이스
/// </summary>
public interface IDamageable
{
    void TakeDamage(float damage);
    void TakeHeal(float hp);
}