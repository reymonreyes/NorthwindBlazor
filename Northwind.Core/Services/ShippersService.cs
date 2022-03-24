using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Interfaces.Repositories;
using Northwind.Core.Interfaces.Services;
using Northwind.Core.Interfaces.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Northwind.Core.Services
{
    public class ShippersService : IShippersService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IShipperValidator _shipperValidator;
        public ShippersService(IUnitOfWork unitOfWork, IShipperValidator shipperValidator)
        {
            _unitOfWork = unitOfWork;
            _shipperValidator = shipperValidator;
        }

        public async Task<ServiceResult> Create(ShipperDto? shipperDto)
        {
            if(shipperDto is null)
                throw new ArgumentNullException("shipper");
            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };

            var validationResult = _shipperValidator.Validate(shipperDto);
            if(validationResult?.Count > 0)
            {
                result.IsSuccessful = false;
                result.Messages.AddRange(validationResult);
            }

            if (!result.IsSuccessful)
                return result;

            var shipper = new Shipper
            {
                Name = shipperDto.Name,
                Phone = shipperDto.Phone,
            };

            await _unitOfWork.ShippersRepository.Create(shipper);
            await _unitOfWork.Commit();

            result.Messages.Clear();
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", shipper.Id.ToString()) });

            return result;
        }

        public async Task<ICollection<ShipperDto>> GetAll()
        {
            ICollection<ShipperDto> result = new List<ShipperDto>();
            var shippers = await _unitOfWork.ShippersRepository.GetAll();
            return shippers.Select(x => new ShipperDto
            {
                Id = x.Id,
                Name = x.Name,
                Phone = x.Phone
            }).ToList();
        }
    }
}
