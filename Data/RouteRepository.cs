using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using RouteAPI.Models;

namespace RouteAPI.Data
{
    public class RouteRepository : IRouteRepository
    {
        private readonly DataContext _context;
        public RouteRepository(DataContext context)
        {
            _context = context;

        }
        public void add<T>(T entity) where T : class
        {
            _context.Add(entity); ;
        }

        public void delete<T>(T entity) where T : class
        {
            _context.Remove(entity);
        }
        public async Task<IEnumerable<Delivery>> getUnassignDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer).Where(d => d.status == "unassign" && d.transDate == tempDate).ToListAsync();
            return delivery;
        }
        public async Task<IEnumerable<Delivery>> getWaitToSendDelivery(string transdate, string carcode)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer).Where(d => d.status == "รอส่ง" && d.transDate == tempDate && d.carCode == carcode).ToListAsync();
            return delivery;
        }

        public async Task<Delivery> getDelivery(string id)
        {
            var delivery = await _context.Delivery.Include(c => c.Customer).FirstOrDefaultAsync(d => d.deliveryId == id);
            return delivery;
        }
        public async Task<bool> saveAll()
        {
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<IEnumerable<Delivery>> getDescUnassignDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);

            var delivery = await _context.Delivery.Include(c => c.Customer)
                .Where(d => d.status == "unassign" && d.transDate == tempDate && d.trip == null)
                .OrderByDescending(d => d.quantity)
                .ToListAsync();
            return delivery;
        }

        public async Task<IEnumerable<Delivery>> getFirstTripDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer)
                .Where(d => d.status == "unassign" && d.transDate == tempDate && d.trip == "1")
                .OrderByDescending(d => d.quantity)
                .ToListAsync();
            return delivery;
        }

        public async Task<IEnumerable<Delivery>> getSecondTripDelivery(string transdate)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery.Include(c => c.Customer)
                .Where(d => d.status == "unassign" && d.transDate == tempDate && d.trip == "2")
                .OrderByDescending(d => d.quantity)
                .ToListAsync();
            return delivery;
        }

        public async Task<IEnumerable<Car>> getCar(string zondId)
        {
            var car = await _context.Car
                .Where(z => z.zoneId == zondId && z.status == "available")
                .OrderBy(c => c.carCode)
                .ToListAsync();
            return car;
        }

        public async Task<IEnumerable<Car>> getCarDesc(string zondId)
        {
            var car = await _context.Car
                .Where(z => z.zoneId == zondId && z.status == "available")
                .OrderByDescending(c => c.carCode)
                .ToListAsync();
            return car;
        }

        public async Task<IEnumerable<Delivery>> getCarDelivery(string transdate, string carCode)
        {
            var tempDate = DateTime.Parse(transdate);
            var delivery = await _context.Delivery
                .Include(c => c.Customer)
                .Where(d => d.status == "รอส่ง" && d.transDate == tempDate && d.carCode == carCode)
                .ToListAsync();
            return delivery;
        }

        public async Task<Warehouse> getWarehouseGps(string warehoseId)
        {
            var warehouse = await _context.Warehouse.FirstOrDefaultAsync(w => w.warehouseId == warehoseId);
            return warehouse;
        }

        public async Task<Car> addNewCar(Car car)
        {
            await _context.Car.AddAsync(car);
            await _context.SaveChangesAsync();
            return car;
        }
        public async Task<bool> CarExists(string carCode)
        {
            if (await _context.Car.AnyAsync(c => c.carCode == carCode))
            {
                return true;
            }
            return false;
        }

        public async Task<string> getLatestCarCode(string zondId)
        {
           string carCode = await _context.Car.Where(z => z.zoneId == zondId).MaxAsync(c => c.carCode);
           return carCode;
        }

        public async Task<Car> searchCar(string carCode)
        {
            var car = await _context.Car.FirstOrDefaultAsync(c => c.carCode == carCode);
            return car;
        }
    }
}