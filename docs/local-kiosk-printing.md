# چاپ فاکتور روی چاپگر کنار هر دستگاه

در صفحه پرینت فاکتور، دکمه چاپ فقط `PrintPreview` را برای رزروهای همان کاربر
باز می‌کند. سرور مالکیت رزرو، سهمیه و ساعت مجاز چاپ را کنترل می‌کند و سپس
مرورگر همان دستگاه `window.print()` را اجرا می‌کند. درخواست چاپ این دکمه به
چاپگر نصب‌شده روی IIS فرستاده نمی‌شود. QR تحویل برای سهمیه‌های قدیمی که
`QoutaPerson` دارند، مانند چاپ قبلی سرور روی فاکتور درج می‌شود.

## آماده‌سازی هر دستگاه ویندوزی

1. چاپگر حرارتی متصل به همان دستگاه را نصب کنید و یک صفحه آزمایشی از ویندوز
   چاپ کنید. اندازه کاغذ را در تنظیمات درایور روی رول ۵۸ میلی‌متری قرار دهید.
2. در پروفایل Chrome مورد استفاده کیوسک، چاپگر متصل به همان دستگاه را به
   عنوان مقصد چاپ انتخاب کنید. مطمئن شوید «Save as PDF» مقصد انتخابی نیست.
3. برای چاپ با یک کلیک و بدون پنجره تأیید، در Chrome نسخه ۱۴۴ یا جدیدتر
   سیاست `SilentPrintingEnabled` را برای آن دستگاه فعال کنید. در PowerShell
   با دسترسی مدیر:

   ```powershell
   New-Item 'HKLM:\Software\Policies\Google\Chrome' -Force | Out-Null
   New-ItemProperty 'HKLM:\Software\Policies\Google\Chrome' -Name SilentPrintingEnabled -PropertyType DWord -Value 1 -Force | Out-Null
   ```

   Chrome را کامل ببندید و دوباره باز کنید؛ در `chrome://policy` مقدار سیاست
   را بررسی کنید. هر دستگاه چاپگر خودش را انتخاب می‌کند و نام چاپگر مرکزی
   `Printer:FoodTokenPrinterName` برای این دکمه استفاده نمی‌شود.

چاپ مرورگر و سیاست Chrome تضمین نمی‌کنند کاغذ واقعاً از چاپگر خارج شده است.
اگر دستگاه قطع یا کاغذ تمام باشد، صف و وضعیت چاپگر همان دستگاه را بررسی کنید.

منبع رفتار سیاست: [مستندات Chromium برای SilentPrintingEnabled](https://chromium.googlesource.com/chromium/src/+/refs/heads/main/components/policy/resources/templates/policy_definitions/Miscellaneous/SilentPrintingEnabled.yaml).
