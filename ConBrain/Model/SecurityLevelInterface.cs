namespace ConBrain.Model
{
    /// <summary>
    /// Уровень секретности данных
    /// </summary>
    public enum SecurityLevel
    {
        Public = 0,
        OnlyFriends = 1,
        Private = 2
    }

    /// <summary>
    /// Объект с параметрами секретности для пользователя
    /// </summary>
    public interface SecurityLevelObject
    {
        public Person Owner { get; }
        public SecurityLevel SecurityLevel { get; }
    }

    internal interface SecurityLevelStrategy
    {
        /// <summary>
        /// Фукнция определения возможности доступа к данным
        /// </summary>
        /// <param name="person">пользователь, от которого производится запрос</param>
        /// <param name="obj">Запрашиваемый объект</param>
        /// <returns>Можно ли выдавать объект пользователю</returns>
        public bool CanGetObject(Person? person, SecurityLevelObject obj);
    }
    internal class PublicSecurityLevelStrategy : SecurityLevelStrategy
    {
        public bool CanGetObject(Person? person, SecurityLevelObject obj)
        {
            return true;
        }
    }
    internal class OnlyFriendsSecurityLevelStrategy : SecurityLevelStrategy
    {
        public bool CanGetObject(Person? person, SecurityLevelObject obj)
        {
            return person != null && obj.Owner.Friends.Any(i=>i.Friend.Id == person.Id);
        }
    }
    internal class PrivateSecurityLevelStrategy : SecurityLevelStrategy
    {
        public bool CanGetObject(Person? person, SecurityLevelObject obj)
        {
            return person?.Id == obj.Owner.Id;
        }
    }

    public static class SecurityLevelObjectExtension
    {
        /// <summary>
        /// Можно ли получить доступ к данным объекта
        /// </summary>
        /// <param name="securityLevel"></param>
        /// <param name="visitor">Пользователь, который совершает запрос</param>
        /// <returns></returns>
        public static bool CanGet(this SecurityLevelObject securityLevel, Person? visitor)
        {
            SecurityLevelStrategy strategy = securityLevel.SecurityLevel switch
            {
                SecurityLevel.Private => new PrivateSecurityLevelStrategy(),
                SecurityLevel.OnlyFriends => new OnlyFriendsSecurityLevelStrategy(),
                _=> new PublicSecurityLevelStrategy()
            };
            return strategy.CanGetObject(visitor, securityLevel);
        }
    }
}
