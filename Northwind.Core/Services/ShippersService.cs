using Northwind.Common.Extensions;
using Northwind.Core.Dtos;
using Northwind.Core.Entities;
using Northwind.Core.Exceptions;
using Northwind.Core.Extensions;
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

        public async Task<ServiceResult> Create(ShipperDto shipperDto)
        {
            if(shipperDto is null)
                throw new ArgumentNullException("shipper");

            Validate(shipperDto);

            var shipper = new Shipper
            {
                Name = shipperDto.Name,
                Phone = shipperDto.Phone,
                ContactName = shipperDto.ContactName
            };

            await _unitOfWork.Start();
            await _unitOfWork.ShippersRepository.Create(shipper);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", shipper.Id.ToString()) });

            return result;
        }

        private void Validate(ShipperDto shipper)
        {
            var validationResult = _shipperValidator.Validate(shipper);
            if (validationResult?.Count > 0)
                throw new ValidationFailedException(validationResult);
        }

        public async Task<ServiceResult> Update(int shipperId, ShipperDto shipperDto)
        {
            if (shipperDto is null)
                throw new ArgumentNullException("shipperDto");
            if(shipperId <= 0)
                throw new ArgumentOutOfRangeException(nameof(shipperId));

            Validate(shipperDto);

            await _unitOfWork.Start();
            var shipper = await _unitOfWork.ShippersRepository.Get(shipperId);
            if (shipper is null) 
                throw new DataNotFoundException("Shipper not found");

            shipper.Name = shipperDto.Name;
            shipper.Phone = shipperDto.Phone;
            shipper.ContactName = shipperDto.ContactName;
            await _unitOfWork.ShippersRepository.Update(shipper);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();

            var result = new ServiceResult { IsSuccessful = true, Messages = new List<ServiceMessageResult>() };
            result.Messages.Add(new ServiceMessageResult { MessageType = Enums.ServiceMessageType.Info, Message = new KeyValuePair<string, string>("Id", shipper.Id.ToString()) });

            return result; 
        }

        public async Task<ShipperDto?> Get(int shipperId)
        {
            if (shipperId <= 0)
                return null;

            await _unitOfWork.Start();
            var shipper = await _unitOfWork.ShippersRepository.Get(shipperId);
            await _unitOfWork.Stop();

            if (shipper == null)
                return null;

            return shipper.ToShipperDto();
        }

        public async Task<ICollection<ShipperDto>> GetAll()
        {
            ICollection<ShipperDto> result = new List<ShipperDto>();
            await _unitOfWork.Start();
            var shippers = await _unitOfWork.ShippersRepository.GetAll();
            await _unitOfWork.Stop();

            return shippers.Select(x => x.ToShipperDto()).ToList();
        }

        public async Task Delete(int shipperId)
        {
            await _unitOfWork.Start();
            await _unitOfWork.ShippersRepository.Delete(shipperId);
            await _unitOfWork.Commit();
            await _unitOfWork.Stop();
        }

        public async Task<IEnumerable<ShipperDto>> FindAsync(string shipperName)
        {
            if (shipperName.IsEmptyOrLengthLessThan(2))
                return new List<ShipperDto>();

            await _unitOfWork.Start();
            var matchedShippers = await _unitOfWork.ShippersRepository.FindAsync(shipperName);
            await _unitOfWork.Stop();

            return matchedShippers.Select(x => new ShipperDto { Id = x.Id, Name = x.Name, });
        }
    }
}
