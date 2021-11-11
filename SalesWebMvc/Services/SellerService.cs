﻿using SalesWebMvc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SalesWebMvc.Services.Exceptions;

namespace SalesWebMvc.Services
{
    public class SellerService
    {
        private readonly SalesWebMvcContext _context;

        public SellerService(SalesWebMvcContext context)
        {
            _context = context;
        }
        

        public async Task<List<Seller>> FindAllAsync() // retorna uma lista do banco de vendedores
        {
            return await _context.Seller.ToListAsync();
        }

        public async Task InsertAsync(Seller obj) // insere no banco um novo vendedor
        {
            _context.Add(obj);
            await _context.SaveChangesAsync();
        }

        public async Task<Seller> FindByIdAsync(int id)
        {
            return await _context.Seller.Include(obj => obj.Department).FirstOrDefaultAsync(obj => obj.Id == id);
        }

        public async Task RemoveAsync(int id)
        {
            try
            {
                var obj = await _context.Seller.FindAsync(id);
                _context.Seller.Remove(obj);
                await _context.SaveChangesAsync();
            }
            catch(DbUpdateException)
            {
                throw new IntegrityException("Can't delete seller because he/she has sales");
            }


            
        }

        public async Task UpdateAsync(Seller obj)
        {
            bool hasAny = await _context.Seller.AnyAsync(x => x.Id == obj.Id);
            if (!hasAny) // verificando se existe no banco o obj com o id passado por parametro
            {
                throw new NotFoundException("Id not found!");
            }
            try
            {
                _context.Seller.Update(obj);
                await _context.SaveChangesAsync();
            }
            catch(DbConcurrencyException e) // Quando chama o Update no banco, ele pode retornar um exception de conflito de
                                             // concorrencia, vamos usar o try para tentar e usar o catch para uma possivel
                                             // captura de erro
            {
                throw new DbConcurrencyException(e.Message);
            }

        }


    }
}
