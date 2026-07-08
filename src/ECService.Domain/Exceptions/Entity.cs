namespace ECService.Domain.Models;

/// <summary>
/// すべてのエンティティが継承する抽象基底クラス
/// ドメイン駆動設計における「エンティティの同一性」を一元的に提供する
/// </summary>
public abstract class Entity
{
    /// <summary>
    /// 同一性判定に用いる識別子(UUID)
    /// </summary>
    protected abstract string Identity { get; }

    public override bool Equals(object? obj)
    {
        if (obj is null || obj.GetType() != GetType())
        {
            return false;
        }

        var other = (Entity)obj;

        if (string.IsNullOrEmpty(Identity) || string.IsNullOrEmpty(other.Identity))
        {
            return ReferenceEquals(this, other);
        }

        return Identity == other.Identity;
    }

    public override int GetHashCode()
    {
        return string.IsNullOrEmpty(Identity)
            ? base.GetHashCode()
            : Identity.GetHashCode();
    }

    public static bool operator ==(Entity? left, Entity? right)
    {
        if (left is null && right is null)
        {
            return true;
        }

        if (left is null || right is null)
        {
            return false;
        }

        return left.Equals(right);
    }

    public static bool operator !=(Entity? left, Entity? right)
    {
        return !(left == right);
    }
}