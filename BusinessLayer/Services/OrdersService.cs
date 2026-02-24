using AutoMapper;
using BusinessLogicLayer.DTO;
using BusinessLogicLayer.ServiceContracts;
using DataAccessLayer.Entities;
using DataAccessLayer.RepositoryContracts;
using FluentValidation;
using FluentValidation.Results;
using MongoDB.Driver;

namespace BusinessLogicLayer.Services
{
    public class OrdersService(IOrderRepository orderRepository,IMapper mapper,IValidator<OrderAddRequest> orderAddRequestValidator
        , IValidator<OrderItemAddRequest> orderItemAddRequestValidator, IValidator<OrderUpdateRequest> orderUpdateRequestValidator
        , IValidator<OrderItemUpdateRequest> orderItemUpdateRequestValidator) : IOrderService
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IMapper _mapper = mapper;
        private readonly IValidator<OrderAddRequest> _orderAddRequestValidator = orderAddRequestValidator;
        private readonly IValidator<OrderItemAddRequest> _orderItemAddRequestValidator = orderItemAddRequestValidator;
        private readonly IValidator<OrderUpdateRequest> _orderUpdateRequestValidator = orderUpdateRequestValidator;
        private readonly IValidator<OrderItemUpdateRequest> _orderItemUpdateRequestValidator = orderItemUpdateRequestValidator;

        public async Task<OrderResponse?> AddOrder(OrderAddRequest orderAddRequest)
        {
            if(orderAddRequest == null) throw new ArgumentNullException(nameof(orderAddRequest));

            //validation using fluent
            ValidationResult orderAddValidationResult = await _orderAddRequestValidator.ValidateAsync(orderAddRequest);

            if(!orderAddValidationResult.IsValid)
            {
                string errors = string.Join(", ",
                    orderAddValidationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors); 
            }

            //validator order item using fluent
            foreach(OrderItemAddRequest orderItemAddRequest in orderAddRequest.OrderItems)
            {
                ValidationResult orderItemAddValidationResult = await 
                    _orderItemAddRequestValidator.ValidateAsync(orderItemAddRequest);

                if(! orderItemAddValidationResult.IsValid)
                {
                    string errors = string.Join(", ",
                    orderItemAddValidationResult.Errors.Select(temp => temp.ErrorMessage));
                    throw new ArgumentException(errors);
                }
            }

            //Add logic for checking if userID exists in users microservice


            //Convert data from req to destination
            Order orderInput = _mapper.Map<Order>(orderAddRequest);

            //Generate values
            foreach (OrderItem orderItem in orderInput.OrderItems)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.TotalPrice;
            }
            orderInput.TotalBill = orderInput.OrderItems.Sum(temp =>
                            temp.TotalPrice);

            Order? addedOrder =  await _orderRepository.AddOrder(orderInput);

            if(addedOrder == null) return null;

            OrderResponse addedRespnse  = _mapper.Map<OrderResponse>(addedOrder);

            return addedRespnse;                    

        }

        public async Task<bool> DeleteOrder(Guid orderID)
        {

            FilterDefinition<Order> filter =  Builders<Order>.Filter.Eq(temp => temp.OrderID ,orderID);

            Order? existingOrder =  await _orderRepository.GetOrderByCondition(filter);
            if (existingOrder == null)
            {
                return false;
            }
            bool isDelete = await _orderRepository.DeleteOrder(orderID);
            return isDelete;
        }

        public async Task<OrderResponse?> GetOrderByCondition(FilterDefinition<Order> filter)
        {
            Order? orrder = await _orderRepository.GetOrderByCondition(filter);
            if (orrder == null)
            {
                return null;
            } 
            return _mapper.Map<OrderResponse>(orrder);
        }

        public async Task<List<OrderResponse?>> GetOrders()
        {
            IEnumerable<Order?> orders = await _orderRepository.GetOrdersAsync();
            IEnumerable<OrderResponse?> orderResponse = _mapper.Map<IEnumerable<OrderResponse?>>(orders);
            return orderResponse.ToList();
        }

        public async Task<List<OrderResponse?>> GetOrdersByCondition(FilterDefinition<Order> filter)
        {
            IEnumerable<Order?> orders = await _orderRepository.GetOrdersByCondition(filter);

            IEnumerable<OrderResponse?> orderResponse = _mapper.Map<IEnumerable<OrderResponse?>>(orders);
            return orderResponse.ToList();
        }

        public async Task<OrderResponse?> UpdateOrder(OrderUpdateRequest orderUpdateRequest)
        {
            if (orderUpdateRequest == null) throw new ArgumentNullException(nameof(orderUpdateRequest));

            //validation using fluent
            ValidationResult orderUpdateValidationResult = await _orderUpdateRequestValidator.ValidateAsync(orderUpdateRequest);

            if (!orderUpdateValidationResult.IsValid)
            {
                string errors = string.Join(", ",
                    orderUpdateValidationResult.Errors.Select(temp => temp.ErrorMessage));
                throw new ArgumentException(errors);
            }

            //validator order item using fluent
            foreach (OrderItemUpdateRequest orderItemUpdateRequest in orderUpdateRequest.OrderItems)
            {
                ValidationResult orderItemUpdateValidationResult = await
                    _orderItemUpdateRequestValidator.ValidateAsync(orderItemUpdateRequest);

                if (!orderItemUpdateValidationResult.IsValid)
                {
                    string errors = string.Join(", ",
                    orderItemUpdateValidationResult.Errors.Select(temp => temp.ErrorMessage));
                    throw new ArgumentException(errors);
                }
            }

            //Add logic for checking if userID exists in users microservice


            //Convert data from req to destination
            Order orderInput = _mapper.Map<Order>(orderUpdateRequest);

            //Generate values
            foreach (OrderItem orderItem in orderInput.OrderItems)
            {
                orderItem.TotalPrice = orderItem.Quantity * orderItem.TotalPrice;
            }
            orderInput.TotalBill = orderInput.OrderItems.Sum(temp =>
                            temp.TotalPrice);

            Order? updatedOrder = await _orderRepository.UpdateOrder(orderInput);

            if (updatedOrder == null) return null;

            OrderResponse addedRespnse = _mapper.Map<OrderResponse>(updatedOrder);

            return addedRespnse;
        }
    }
}
