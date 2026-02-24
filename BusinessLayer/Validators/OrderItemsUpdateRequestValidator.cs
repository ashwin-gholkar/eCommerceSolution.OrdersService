using BusinessLogicLayer.DTO;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLogicLayer.Validators
{
    public class OrderItemsUpdateRequestValidator : AbstractValidator<OrderItemUpdateRequest>
    {
        public OrderItemsUpdateRequestValidator()
        {

            RuleFor(temp => temp.ProductID)
                .NotEmpty().WithErrorCode("ProductID cant be blank");
            RuleFor(temp => temp.UnitPrice)
                .NotEmpty().WithErrorCode("UnitPrice cant be blank")
                .GreaterThan(0).WithErrorCode("Unit price cant be less than or equal to zero");
                RuleFor(temp => temp.Quantity)
                    .NotEmpty().WithErrorCode("Order must contain at least one OrderItem.");

        }
    }
}
