﻿namespace Cart.Domain.Shared
{
    public class Entity<TKey>
    {
        public TKey Id { get; set; }

        protected bool Equals(Entity<TKey> other) => Id.Equals(other.Id);

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Entity<TKey>) obj);
        }

        public override int GetHashCode() => Id.GetHashCode();
    }
}