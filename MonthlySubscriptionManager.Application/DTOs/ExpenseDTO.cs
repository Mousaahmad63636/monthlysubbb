﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuickTechSystems.Application.DTOs
{
    public class ExpenseDTO : BaseDTO
    {
        public int ExpenseId { get; set; }
        public string Reason { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime Date { get; set; }
        public string? Notes { get; set; }
        public string Category { get; set; } = string.Empty;
        public bool IsRecurring { get; set; }
    }
}
