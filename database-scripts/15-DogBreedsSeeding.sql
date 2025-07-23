-- Dog Breeds Seeding Data Script
-- Comprehensive list of dog breeds for autocomplete functionality
-- Includes common breeds, size categories, and alternative names

-- Create DogBreeds table if it doesn't exist
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'DogBreeds')
BEGIN
    CREATE TABLE DogBreeds (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Name nvarchar(100) NOT NULL,
        AlternativeNames nvarchar(500) NULL,
        SizeCategory nvarchar(20) NULL,
        IsCommon bit NOT NULL DEFAULT 1
    );
END

-- Clear existing data
DELETE FROM DogBreeds;

-- Insert comprehensive dog breed data
INSERT INTO DogBreeds (Name, AlternativeNames, SizeCategory, IsCommon) VALUES
-- Very Common Large Breeds
('Labrador Retriever', 'Lab, Labrador', 'Large', 1),
('Golden Retriever', 'Golden', 'Large', 1),
('German Shepherd', 'German Shepherd Dog, GSD, Alsatian', 'Large', 1),
('Bulldog', 'English Bulldog', 'Medium', 1),
('Poodle', 'Standard Poodle, Miniature Poodle, Toy Poodle', 'Medium', 1),
('Beagle', NULL, 'Medium', 1),
('Rottweiler', 'Rottie', 'Large', 1),
('Yorkshire Terrier', 'Yorkie, York', 'Small', 1),
('Dachshund', 'Wiener Dog, Sausage Dog, Doxie', 'Small', 1),
('Siberian Husky', 'Husky', 'Large', 1),

-- Common Medium Breeds
('Australian Shepherd', 'Aussie', 'Medium', 1),
('Boxer', NULL, 'Large', 1),
('Border Collie', NULL, 'Medium', 1),
('Boston Terrier', NULL, 'Small', 1),
('Cocker Spaniel', 'English Cocker Spaniel, American Cocker Spaniel', 'Medium', 1),
('Shih Tzu', NULL, 'Small', 1),
('Chihuahua', NULL, 'Small', 1),
('Pomeranian', 'Pom', 'Small', 1),
('French Bulldog', 'Frenchie', 'Small', 1),
('Maltese', NULL, 'Small', 1),

-- Common Working/Sporting Breeds
('Great Dane', NULL, 'Giant', 1),
('Mastiff', 'English Mastiff', 'Giant', 1),
('Saint Bernard', 'St. Bernard', 'Giant', 1),
('Doberman Pinscher', 'Doberman, Dobie', 'Large', 1),
('Weimaraner', NULL, 'Large', 1),
('Vizsla', NULL, 'Medium', 1),
('Pointer', 'English Pointer, German Shorthaired Pointer', 'Medium', 1),
('Setter', 'Irish Setter, English Setter, Gordon Setter', 'Large', 1),
('Spaniel', 'Springer Spaniel, Field Spaniel', 'Medium', 1),
('Retriever', 'Chesapeake Bay Retriever, Flat-Coated Retriever', 'Large', 1),

-- Terrier Breeds
('Jack Russell Terrier', 'JRT, Parson Russell Terrier', 'Small', 1),
('Bull Terrier', NULL, 'Medium', 1),
('Scottish Terrier', 'Scottie', 'Small', 1),
('West Highland White Terrier', 'Westie', 'Small', 1),
('Fox Terrier', 'Wire Fox Terrier, Smooth Fox Terrier', 'Small', 1),
('Airedale Terrier', 'King of Terriers', 'Large', 1),
('Staffordshire Bull Terrier', 'Staffy', 'Medium', 1),
('American Pit Bull Terrier', 'Pit Bull, APBT', 'Medium', 1),
('American Staffordshire Terrier', 'AmStaff', 'Medium', 1),
('Cairn Terrier', NULL, 'Small', 1),

-- Spitz and Nordic Breeds
('Alaskan Malamute', 'Mal', 'Large', 1),
('Samoyed', NULL, 'Large', 1),
('Chow Chow', NULL, 'Medium', 1),
('Finnish Spitz', NULL, 'Medium', 0),
('Norwegian Elkhound', NULL, 'Medium', 0),
('Keeshond', NULL, 'Medium', 0),
('Akita', 'Akita Inu, American Akita', 'Large', 1),
('Shiba Inu', NULL, 'Small', 1),

-- Herding Breeds
('Australian Cattle Dog', 'Blue Heeler, Red Heeler', 'Medium', 1),
('Belgian Shepherd', 'Belgian Malinois, Belgian Tervuren', 'Large', 0),
('Old English Sheepdog', NULL, 'Large', 0),
('Collie', 'Rough Collie, Smooth Collie', 'Large', 1),
('Shetland Sheepdog', 'Sheltie', 'Small', 1),
('Welsh Corgi', 'Pembroke Welsh Corgi, Cardigan Welsh Corgi', 'Small', 1),

-- Hound Breeds
('Bloodhound', NULL, 'Large', 1),
('Basset Hound', NULL, 'Medium', 1),
('Greyhound', NULL, 'Large', 1),
('Whippet', NULL, 'Medium', 1),
('Afghan Hound', NULL, 'Large', 0),
('Borzoi', 'Russian Wolfhound', 'Large', 0),
('Irish Wolfhound', NULL, 'Giant', 0),
('Saluki', NULL, 'Large', 0),
('Rhodesian Ridgeback', NULL, 'Large', 0),
('Coonhound', 'Black and Tan Coonhound, Redbone Coonhound', 'Large', 0),

-- Toy Breeds
('Pug', NULL, 'Small', 1),
('Cavalier King Charles Spaniel', 'Cavalier', 'Small', 1),
('Havanese', NULL, 'Small', 1),
('Papillon', NULL, 'Small', 0),
('Chinese Crested', NULL, 'Small', 0),
('Italian Greyhound', NULL, 'Small', 0),
('Japanese Chin', NULL, 'Small', 0),
('Pekingese', NULL, 'Small', 0),
('Toy Fox Terrier', NULL, 'Small', 0),
('Affenpinscher', NULL, 'Small', 0),

-- Less Common but Notable Breeds
('Bernese Mountain Dog', 'Berner', 'Large', 1),
('Great Pyrenees', NULL, 'Giant', 0),
('Newfoundland', 'Newf', 'Giant', 0),
('Portuguese Water Dog', NULL, 'Medium', 0),
('Standard Schnauzer', 'Schnauzer', 'Medium', 1),
('Miniature Schnauzer', NULL, 'Small', 1),
('Giant Schnauzer', NULL, 'Large', 0),
('Bichon Frise', NULL, 'Small', 1),
('Lhasa Apso', NULL, 'Small', 0),
('Tibetan Terrier', NULL, 'Medium', 0),

-- Rare and Unique Breeds
('Basenji', NULL, 'Small', 0),
('Xoloitzcuintli', 'Mexican Hairless Dog, Xolo', 'Medium', 0),
('Pharaoh Hound', NULL, 'Medium', 0),
('Azawakh', NULL, 'Large', 0),
('Lundehund', 'Norwegian Lundehund', 'Small', 0),
('Telomian', NULL, 'Medium', 0),
('Carolina Dog', 'American Dingo', 'Medium', 0),
('Catalburun', NULL, 'Medium', 0),
('Mudi', NULL, 'Medium', 0),
('Lagotto Romagnolo', NULL, 'Medium', 0),

-- Mixed/Designer Breeds (Common)
('Labradoodle', 'Labrador Poodle Mix', 'Large', 1),
('Goldendoodle', 'Golden Retriever Poodle Mix', 'Large', 1),
('Cockapoo', 'Cocker Spaniel Poodle Mix', 'Small', 1),
('Puggle', 'Pug Beagle Mix', 'Small', 1),
('Schnoodle', 'Schnauzer Poodle Mix', 'Medium', 1),
('Yorkipoo', 'Yorkshire Terrier Poodle Mix', 'Small', 1),
('Maltipoo', 'Maltese Poodle Mix', 'Small', 1),
('Bernedoodle', 'Bernese Mountain Dog Poodle Mix', 'Large', 1),
('Saint Berdoodle', 'Saint Bernard Poodle Mix', 'Giant', 1),
('Mixed Breed', 'Mutt, Crossbreed, Mongrel', 'Medium', 1);

-- Add index for better search performance
CREATE NONCLUSTERED INDEX IX_DogBreeds_Name ON DogBreeds (Name);
CREATE NONCLUSTERED INDEX IX_DogBreeds_IsCommon ON DogBreeds (IsCommon);

-- Verify the data
SELECT COUNT(*) as 'Total Dog Breeds Inserted' FROM DogBreeds;
SELECT SizeCategory, COUNT(*) as 'Count' FROM DogBreeds GROUP BY SizeCategory;
SELECT IsCommon, COUNT(*) as 'Count' FROM DogBreeds GROUP BY IsCommon;

PRINT 'Dog breeds seeding completed successfully!';