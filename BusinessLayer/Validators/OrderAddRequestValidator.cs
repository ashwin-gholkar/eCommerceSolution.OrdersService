using BusinessLogicLayer.DTO;
using FluentValidation;

namespace BusinessLogicLayer.Validators
{
    public class OrderAddRequestValidator:AbstractValidator<OrderAddRequest>
    {

        public OrderAddRequestValidator()
        {
            RuleFor(temp => temp.UserID)
                .NotEmpty().WithErrorCode("UserId cant be blank");
            RuleFor(temp => temp.OrderDate)
                .NotEmpty().WithErrorCode("OrderDate cant be blank");
            RuleFor(temp => temp.OrderItems)
                .Must(orderItems => orderItems != null && orderItems.Count > 0)
                .WithErrorCode("Order must contain at least one OrderItem.");

        }
    }
}
