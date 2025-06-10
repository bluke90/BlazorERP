# Params
$connectionString = ''
$contextDir = "Data/Context"
$outputDir = "Data/Entities"
$contextName = "proContext"

# Run Scaffold
dotnet ef dbcontext scaffold --no-onconfiguring --use-database-names --verbose $connectionString "Microsoft.EntityFrameworkCore.SqlServer" --output-dir $outputDir --context-dir $contextDir --context $contextName --force --no-build

# Set path to database context
$dbContextFile = Join-Path $contextDir "$contextName.cs"

# Custom OnConfiguring method
$customOnConfiguring = @"
    private readonly IConfiguration _configuration;
    
    public proContext(DbContextOptions<proContext> options) : base(options) {}
    
    public proContext() {}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var configuration = Program.Configuration;
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
        
        optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
    } 
"@

# Use regex to find the constructor
$constructorRegex = "public\s+\w+\s*\(\s*DbContextOptions<\w+>\s*\w*\s*\)\s*:\s*base\(.*?\)\s*\{[^}]*\}"

# Get Content
$content = Get-Content $dbContextFile -Raw

# Add Microsoft.Extensions.Configuration if it doesnt exist
if ($content -notmatch "using Microsoft\.Extensions\.Configuration;") {
    Write-Host "Adding 'using Microsoft.Extensions.Configuration;' to the top of the file."
    $content = "using Microsoft.Extensions.Configuration;" + $content
}

if ($content -match $constructorRegex) {
    Write-Host "Constructor found. Replacing with OnConfiguring override method."
    $content = $content -replace $constructorRegex, $customOnConfiguring
    Write-Host "Post Processing Completed..."
} else {
    Write-Host "Constructor not found. No changes made."
}

# Write the modified content back to the file
Set-Content $dbContextFile $content