# عیب‌یابی چاپ ژتون غذا — IIS / Windows / RONGTA 58mm

## علت اشتباه گرفتن پیام با چاپ فیزیکی

در صفحه `/ShowReserveManagment/ShowReserve/Index` دو مسیر وجود دارد:

1. **چاپ با پرینتر این رایانه:** `PrintPreview` رسید ۵۸ میلی‌متری را در
   مرورگر باز می‌کند و `window.print()` را اجرا می‌کند. کاربر باید
   پرینتر متصل به همان رایانه را در پنجره چاپ انتخاب کند.
2. **ارسال به پرینتر سرور:** درخواست `POST /ShowReserveManagment/ShowReserve/PrintMeal`
   با AntiForgery وارد `FoodReserveManager.PrintFoodReserveAsync` می‌شود.
   در `PrintDocument.Print()` اطلاعات به زیرساخت چاپ سیستم اجراکننده
   IIS ارسال می‌شوند؛ موفقیت در این مرحله **به معنی خروج کاغذ نیست**.

**مایکروسافت صراحتاً درباره استفاده از `System.Drawing.Printing` در
ASP.NET/Windows Services هشدار می‌دهد؛ برای چاپ خودکار و قابل اتکا
بهتر است یک Print Agent/Windows Service مستقل روی همان رایانه چاپگر
(یا چاپ شبکه‌ایِ کنترل‌شده) طراحی شود.**

مرجع:
https://learn.microsoft.com/dotnet/api/system.drawing.printing

## تغییر سورس

- نام ثابت و قدیمی `RONGTA 58mm Series Printer(2)` دیگر به عنوان
  fallback استفاده نمی‌شود؛ مدیر باید نام واقعی چاپگر **روی سرور**
  را در `Printer:FoodTokenPrinterName` تنظیم کند. مقدار خالی
  موجب **رد روشن و ثبت خطا** می‌شود و صف اشتباه را هدف نمی‌گیرد.
- نام چاپگر، نام سرور و حساب واقعی اجرای وب در Logger ثبت می‌شود.
- بعد از انتخاب چاپگر، اندازه کاغذ پیش‌فرض درایور در Logger ثبت می‌شود.
  برای رول ۵۸mm عرض حدود ۲۲۸ در واحد `1/100 inch` است.
- صفحه `/Admin/QuotaControl/PrinterDiagnostics` فقط برای Admin است
  و از **همان فرآیند IIS** فهرست پرینترها، صحت نام و کاغذ را نشان می‌دهد.
  این صفحه هیچ فرمان چاپی ارسال نمی‌کند و به‌تنهایی کاغذ را تأیید نمی‌کند.

## پیکربندی

در فایل تنظیمات مخصوص سرور یا environment variable:

```json
"Printer": {
  "FoodTokenPrinterName": "EXACT NAME AS SHOWN ON IIS HOST"
}
```

یا:

```text
Printer__FoodTokenPrinterName=EXACT NAME AS SHOWN ON IIS HOST
```

نام باید **دقیقاً** برابر نامی باشد که چاپگر برای حساب اجراکننده
سامانه قابل مشاهده است. به نام نمایش‌داده‌شده روی رایانه شخصی
اعتماد نکنید، مگر همان رایانه میزبان IIS باشد.

## دستورهای تشخیص روی همان سرور چاپ

در PowerShell روی سرور IIS:

```powershell
# ۱. سرویس صف ویندوز
Get-Service Spooler

# ۲. چاپگرهای نصب‌شده، پورت و درایور
Get-Printer | Select-Object Name, DriverName, PortName, PrinterStatus, Shared

# ۳. مقصد شبکه یا USB پورت‌ها
Get-PrinterPort | Select-Object Name, PrinterHostAddress, PortNumber

# ۴. صف چاپ چاپگر موردنظر (نام را از مرحله ۲ بردارید)
Get-PrintJob -PrinterName "EXACT PRINTER NAME" -ErrorAction SilentlyContinue |
  Select-Object ID, DocumentName, JobStatus, Size, SubmittedTime

# ۵. فقط در صورت نیاز به بررسی رویداد، لاگ چاپ را با دسترسی مدیر فعال کنید:
# wevtutil sl Microsoft-Windows-PrintService/Operational /e:true
Get-WinEvent -LogName 'Microsoft-Windows-PrintService/Operational' -MaxEvents 20 |
  Select-Object TimeCreated, Id, Message
```

**تشخیص از نتایج:**

- `Get-Service Spooler` غیر از `Running`: ابتدا سرویس را بررسی کنید.
- نام چاپگر در صفحه عیب‌یابی IIS نیست: احتمال نصب چاپگر فقط برای
  Administrator یا روی رایانه دیگر وجود دارد.
- نام موجود است ولی `IsValid=false`: درایور/پورت/حساب اجرای وب را بررسی کنید.
- کار در `Get-PrintJob` باقی می‌ماند: مشکل صف چاپ، پورت، دستگاه یا درایور محتمل است.
- کار از صف ناپدید می‌شود ولی کاغذ خروجی ندارد: به معنی اثبات چاپ نیست؛
  از **Printer Properties → Print Test Page** روی همان سرور استفاده کنید.
- تست‌پیج ویندوز چاپ می‌شود ولی ژتون نمی‌شود: تنظیم Paper Size،
  Font/Graphics و محدودیت‌های GDI داخل IIS را بررسی کنید.
- تست‌پیج فقط از کامپیوتر کاربر چاپ می‌شود نه سرور: دکمه
  «چاپ با پرینتر این رایانه» را استفاده کنید یا یک Print Agent محلی بسازید.

## نکته‌های عملی مهم

- تنظیمات چاپگر حرارتی، `Printer Preferences`، رول کاغذ، درایور
  واقعی RONGTA، پورت USB/شبکه و حساب `ApplicationPoolIdentity`
  باید با یکدیگر هماهنگ باشند.
- اگر `Printer:FoodTokenPrinterName` تنظیم نشده باشد، **چاپ سرور
  عمداً غیرفعال** می‌ماند و دیگر پیغام موفقیت ساختگی نمی‌دهد.
- جواب `Status=true` تنها نشان‌دهنده ارسال در لایه GDI/Spooler است؛
  برای چاپ فیزیکیِ تضمین‌پذیر، باید Print Agent با تأیید وضعیت دستگاه
  و شناسه job، یا راهکار چاپگر متناسب طراحی شود.
- صفحه پیش‌نمایش محلی در حال حاضر **رسید رزرو** است، نه QR
  یک‌بارمصرفِ تأیید تحویل غذا در سیستم جدید.
- ثبت رویدادها در `ILogger` انجام می‌شود؛ `Serilog` داخل `Program.cs`
  فعلاً کامنت است و برای نگهداری پایدار لاگ چاپ روی IIS باید مقصد
  استاندارد و دارای مجوز نوشتن جداگانه تنظیم شود.

## سناریوهای تست پس از انتشار

1. بدون `Printer:FoodTokenPrinterName` روی سرور: درخواست چاپ
   خطای مشخص تنظیمات بدهد و `Status=false` برگرداند.
2. نام اشتباه: `IsValid` یا فهرست چاپگرها خطای روشن بدهد.
3. نام صحیح و پرینتر USB روی سرور: تست‌پیج ویندوز و سپس ژتون سامانه؛
   نتیجه صفحه و صف باید مقایسه شوند.
4. پرینتر USB فقط روی رایانه کلاینت: مسیر چاپ محلی مرورگر را تست کنید؛
   چاپ سرور نباید به اشتباه ادعای خروج کاغذ از کلاینت کند.
5. از مدیر، صفحه `/Admin/QuotaControl/PrinterDiagnostics` را باز
   و نام سرور، حساب اجرای IIS و کاغذ پیش‌فرض را با تنظیمات واقعی تطبیق دهید.
