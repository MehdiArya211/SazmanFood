using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Config
{
    public class RoleMenuConfig : IEntityTypeConfiguration<RoleMenu>
    {
        public void Configure(EntityTypeBuilder<RoleMenu> builder)
        {
            // کلید ترکیبی
            builder.HasKey(x => new { x.RoleId, x.MenuId });

            // نفش
            builder.HasOne(um => um.Role)
                .WithMany(u => u.Menus)
                .HasForeignKey(um => um.RoleId);

            // منو
            builder.HasOne(um => um.Menu)
                .WithMany(u => u.Roles)
                .HasForeignKey(um => um.MenuId);
        }
    }
}
