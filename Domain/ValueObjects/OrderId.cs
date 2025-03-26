using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ValueObjects
{
    // Domain/ValueObjects/OrderId.cs
    public sealed class OrderId : ValueObject
    {
        public Guid Value { get; }

        private OrderId(Guid value)
        {
            if (value == Guid.Empty)
                throw new Exception("Order ID cannot be empty");

            Value = value;
        }

        public static OrderId New() => new(Guid.NewGuid());

        public static OrderId From(Guid value) => new(value);

        public static OrderId From(string value)
        {
            if (!Guid.TryParse(value, out var guid))
                throw new Exception("Invalid Order ID format");

            return new OrderId(guid);
        }

        public static implicit operator Guid(OrderId id) => id.Value;

        public override string ToString() => Value.ToString();

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}
