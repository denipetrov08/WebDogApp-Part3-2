using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebDogApp.Core.Contracts;
using WebDogApp.Infrastructure.Data;
using WebDogApp.Infrastructure.Data.Domain;
using Microsoft.EntityFrameworkCore;

namespace WebDogApp.Core.Services
{
    public class DogService : IDogService
    {
        private readonly ApplicationDbContext _context;

        public DogService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool Create(string name, int age, int breedId, string picture)
        {
            Dog item = new Dog
            {
                Name = name,
                Age = age,
                Breed = _context.Breeds.Find(breedId),
                Picture = picture,
            };

            _context.Dogs.Add(item);
            return _context.SaveChanges() != 0;
        }

        public Dog GetDogById(int dogId)
        {
            return _context.Dogs.Include(d => d.Breed).FirstOrDefault(d => d.Id == dogId);
        }

        public List<Dog> GetDogs()
        {
            return _context.Dogs.Include(d => d.Breed).ToList();
        }

        public List<Dog> GetDogs(string searchStringBreed, string searchStringName)
        {
            List<Dog> dogs = _context.Dogs.Include(d => d.Breed).ToList();

            if (!String.IsNullOrEmpty(searchStringBreed) && !String.IsNullOrEmpty(searchStringName))
            {
                dogs = dogs.Where(d => d.Breed.Name.Contains(searchStringBreed) && d.Name.Contains(searchStringName)).ToList();
            }
            else if (!String.IsNullOrEmpty(searchStringBreed))
            {
                dogs = dogs.Where(d => d.Breed.Name.Contains(searchStringBreed)).ToList();
            }
            else if (!String.IsNullOrEmpty(searchStringName))
            {
                dogs = dogs.Where(d => d.Name.Contains(searchStringName)).ToList();
            }
            return dogs;
        }

        public bool RemoveById(int dogId)
        {
            var dog = GetDogById(dogId);
            if (dog == null)
            {
                return false;
            }
            _context.Remove(dog);
            return _context.SaveChanges() != 0;
        }

        public bool UpdateDog(int dogId, string name, int age, int breedId, string picture)
        {
            var dog = GetDogById(dogId);
            if (dog == null)
            {
                return false;
            }
            dog.Name = name;
            dog.Age = age;
            dog.Breed = _context.Breeds.Find(breedId);
            dog.Picture = picture;
            _context.Update(dog);
            return _context.SaveChanges() != 0;
        }
    }
}
