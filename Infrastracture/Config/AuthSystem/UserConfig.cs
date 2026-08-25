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
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            // یونیک کردن نام کاربری
            builder.HasIndex(x => x.Username).IsUnique(true);


            // نقش
            builder.HasOne(x => x.Role)
                .WithMany(z => z.Users)
                .HasForeignKey(x => x.RoleId);
        }
    }
}
