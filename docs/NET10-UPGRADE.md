# ارتقای سولوشن به .NET 10

تمام ۱۳ پروژه ITOFood.sln روی net10.0 قرار دارند.

## تغییرات
- بسته‌های Microsoft.EntityFrameworkCore، Microsoft.AspNetCore و Microsoft.Extensions به 10.0.12 ارتقا یافتند.
- LinqKit.Microsoft.EntityFrameworkCore به 10.0.11 و Z.EntityFramework.Plus.EFCore به 10.105.8.1 ارتقا یافتند.
- Microsoft.Data.SqlClient به 6.1.6 ارتقا یافت تا حداقل نسخه لازم EF Core SQL Server تأمین شود.
- بسته‌های قدیمی Microsoft.AspNetCore.Http/Mvc با FrameworkReference به Microsoft.AspNetCore.App جایگزین شدند.
- System.Net.Http مستقل حذف شد؛ API آن در چارچوب فعلی موجود است.
- System.Drawing.Common به 10.0.12 ارتقا یافت؛ چاپ و پردازش تصویر آن همچنان نیازمند Windows است.
- چهار استفاده PropertyValues.EntityType در UnitOfWork با EntityEntry.Metadata جایگزین شد. نام نوع برای لاگ از Metadata.ClrType.Name گرفته می‌شود.
- global.json انتخاب SDK پایدار نسخه ۱۰ را با rollForward=latestFeature کنترل می‌کند.
- dotnet-tools.json ابزار محلی dotnet-ef را روی 10.0.12 ثابت می‌کند.
- Razor RuntimeCompilation برای حفظ رفتار فعلی به نسخه ۱۰ ارتقا یافته است. این قابلیت در .NET 10 منسوخ اعلام شده؛ تغییر معماری Viewها در این ارتقا انجام نشده است.

## اجرا
.NET 10 SDK را نصب کنید. Visual Studio باید پشتیبانی .NET 10 داشته باشد.
در پوشه Solution:
```powershell
dotnet --version
dotnet tool restore
dotnet restore ITOFood.sln
dotnet build ITOFood.sln -c Release
dotnet publish BlankProject/Food.csproj -c Release -o ./publish/web
dotnet publish WindowsService.FajrLog/WindowsService.FajrLog.csproj -c Release -o ./publish/worker
```
برای IIS، Hosting Bundle نسخه ۱۰ باید نصب باشد. به دلیل وابستگی‌های مشترک ASP.NET Core، Worker نیز به Microsoft.AspNetCore.App 10 نیاز دارد؛ Hosting Bundle این Runtime را تأمین می‌کند.

هیچ Migration ایجاد یا اعمال نشده است. پیش از هر اقدام روی دیتابیس، وضعیت Migrationها و تغییرات مدل باید روی محیط آزمایشی بررسی شود. ارتقای EF به‌تنهایی به معنی نیاز به اجرای خودکار Update-Database نیست.

## اعتبارسنجی انجام‌شده
- SDK: 10.0.401
- Restore موفق و رفع خطای downgrade مربوط به SqlClient.
- Build Release کل Solution موفق: صفر خطا، ۲۴۶ هشدار در خروجی Build اعتبارسنجی.
- Publish وب و Worker موفق؛ runtimeconfig هر دو net10.0 است.
- ساخت مدل و تولید SQL بدون اتصال به دیتابیس: ApplicationContext با ۴۰ Entity، LogContext با ۲ Entity.
- ابزار محلی dotnet-ef نسخه 10.0.12 اجرا شد.
- git diff --check موفق.

هشدارهای قدیمی مربوط به System.Linq.Dynamic.Core، ImageSharp، System.Data.SqlClient و AngleSharp در وابستگی‌های جانبی باقی‌اند. ارتقای دات‌نت به معنی رفع همه یافته‌های امنیتی گزارش قبلی نیست.
ورود دستگاه واقعی، SignalR، Redis، عملیات SQL و چاپگر با سرویس‌های واقعی آزمون نشده‌اند. Build موفق جایگزین آزمون یکپارچه روی Windows نیست.
