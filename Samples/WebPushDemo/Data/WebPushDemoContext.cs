using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace WebPushDemo.Models
{
    public class WebPushDemoContext : DbContext
    {
        public WebPushDemoContext (DbContextOptions<WebPushDemoContext> options)
            : base(options)
        {
        }

        public DbSet<WebPushDemo.Models.Devices> Devices { get; set; }
    }
}
