dotnet ef dbcontext scaffold "Server=localhost;Port=32769;Database=TIRE_STORE_ALTER;User=root;Password=nhat1997;TreatTinyAsBoolean=true;" "Pomelo.EntityFrameworkCore.MySql" -c StoreContext -o "Domain" -f