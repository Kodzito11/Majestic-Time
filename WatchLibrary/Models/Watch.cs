﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WatchLibrary
{
    public class Watch
    {
        public int Id { get; set; }
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public string? ReferenceNumber { get; set; }
        public int Year { get; set; }
        public string? Functions { get; set; } //Alle ure har ikke nødvendigvis ekstra funktioner
        public int Size { get; set; }
        public string? Description { get; set; }
        public decimal Price { get; set; }

        private void ValidateBrand()
        {
            if (string.IsNullOrWhiteSpace(Brand))
            {
                throw new ArgumentNullException(nameof(Brand), "Please insert a valid Brand");
            }
        }

        private void ValidateModel()
        {
            if (string.IsNullOrWhiteSpace(Model))
            {
                throw new ArgumentNullException(nameof(Model), "Please insert a valid Model");
            }
        }

        private void ValidateReferenceNumber()
        {
            if (string.IsNullOrWhiteSpace(ReferenceNumber))
            {
                throw new ArgumentNullException(nameof(ReferenceNumber), "Please insert a valid Reference Number");
            }
        }

        private void ValidateYear()
        {
            if (Year < 1900 || Year > DateTime.Now.Year)
            {
                throw new ArgumentOutOfRangeException(nameof(Year), "Please insert a valid Year");
            }
        }

        private void ValidateFunctions()
        {
            if (string.IsNullOrWhiteSpace(Functions))
            {
                throw new ArgumentNullException(nameof(Functions), "Please insert a valid Functions");
            }
        }

        public void Validate()
        {
            ValidateBrand();
            ValidateModel();
            ValidateReferenceNumber();
            ValidateYear();
            ValidateFunctions();
        }



        public override string ToString()
        {
            return $"Id: {Id}, Brand: {Brand}, Model: {Model}, ReferenceNumber: {ReferenceNumber}, Year: {Year}, Functions: {Functions}, Size: {Size}";
        }
    }

}
