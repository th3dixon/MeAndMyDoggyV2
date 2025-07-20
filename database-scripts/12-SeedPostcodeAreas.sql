-- =============================================
-- MeAndMyDoggy - Seed UK Postcode Areas
-- =============================================
-- Manually insert all UK postcode areas if import failed

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Seeding UK postcode areas...';
    
    -- Clear existing postcode areas
    DELETE FROM [dbo].[PostcodeAreas];
    
    -- Insert all UK postcode areas
    INSERT INTO [dbo].[PostcodeAreas] ([PostcodeArea], [AreaName], [Region])
    VALUES
        ('AB', 'Aberdeen', 'Scotland'),
        ('AL', 'St Albans', 'East of England'),
        ('B', 'Birmingham', 'West Midlands'),
        ('BA', 'Bath', 'South West'),
        ('BB', 'Blackburn', 'North West'),
        ('BD', 'Bradford', 'Yorkshire'),
        ('BH', 'Bournemouth', 'South West'),
        ('BL', 'Bolton', 'North West'),
        ('BN', 'Brighton', 'South East'),
        ('BR', 'Bromley', 'London'),
        ('BS', 'Bristol', 'South West'),
        ('BT', 'Belfast', 'Northern Ireland'),
        ('CA', 'Carlisle', 'North West'),
        ('CB', 'Cambridge', 'East of England'),
        ('CF', 'Cardiff', 'Wales'),
        ('CH', 'Chester', 'North West'),
        ('CM', 'Chelmsford', 'East of England'),
        ('CO', 'Colchester', 'East of England'),
        ('CR', 'Croydon', 'London'),
        ('CT', 'Canterbury', 'South East'),
        ('CV', 'Coventry', 'West Midlands'),
        ('CW', 'Crewe', 'North West'),
        ('DA', 'Dartford', 'South East'),
        ('DD', 'Dundee', 'Scotland'),
        ('DE', 'Derby', 'East Midlands'),
        ('DG', 'Dumfries', 'Scotland'),
        ('DH', 'Durham', 'North East'),
        ('DL', 'Darlington', 'North East'),
        ('DN', 'Doncaster', 'Yorkshire and the Humber'),
        ('DT', 'Dorchester', 'South West'),
        ('DY', 'Dudley', 'West Midlands'),
        ('E', 'East London', 'London'),
        ('EC', 'East Central London', 'London'),
        ('EH', 'Edinburgh', 'Scotland'),
        ('EN', 'Enfield', 'London'),
        ('EX', 'Exeter', 'South West'),
        ('FK', 'Falkirk', 'Scotland'),
        ('FY', 'Blackpool', 'North West'),
        ('G', 'Glasgow', 'Scotland'),
        ('GL', 'Gloucester', 'South West'),
        ('GU', 'Guildford', 'South East'),
        ('HA', 'Harrow', 'London'),
        ('HD', 'Huddersfield', 'Yorkshire'),
        ('HG', 'Harrogate', 'Yorkshire'),
        ('HP', 'Hemel Hempstead', 'East of England'),
        ('HR', 'Hereford', 'West Midlands'),
        ('HS', 'Hebrides', 'Scotland'),
        ('HU', 'Hull', 'Yorkshire and the Humber'),
        ('HX', 'Halifax', 'Yorkshire'),
        ('IG', 'Ilford', 'London'),
        ('IP', 'Ipswich', 'East of England'),
        ('IV', 'Inverness', 'Scotland'),
        ('KA', 'Kilmarnock', 'Scotland'),
        ('KT', 'Kingston upon Thames', 'London'),
        ('KW', 'Kirkwall', 'Scotland'),
        ('KY', 'Kirkcaldy', 'Scotland'),
        ('L', 'Liverpool', 'North West'),
        ('LA', 'Lancaster', 'North West'),
        ('LD', 'Llandrindod Wells', 'Wales'),
        ('LE', 'Leicester', 'East Midlands'),
        ('LL', 'Llandudno', 'Wales'),
        ('LN', 'Lincoln', 'East Midlands'),
        ('LS', 'Leeds', 'Yorkshire'),
        ('LU', 'Luton', 'East of England'),
        ('M', 'Manchester', 'North West'),
        ('ME', 'Rochester', 'South East'),
        ('MK', 'Milton Keynes', 'South East'),
        ('ML', 'Motherwell', 'Scotland'),
        ('N', 'North London', 'London'),
        ('NE', 'Newcastle upon Tyne', 'North East'),
        ('NG', 'Nottingham', 'East Midlands'),
        ('NN', 'Northampton', 'East Midlands'),
        ('NP', 'Newport', 'Wales'),
        ('NR', 'Norwich', 'East of England'),
        ('NW', 'North West London', 'London'),
        ('OL', 'Oldham', 'North West'),
        ('OX', 'Oxford', 'South East'),
        ('PA', 'Paisley', 'Scotland'),
        ('PE', 'Peterborough', 'East of England'),
        ('PH', 'Perth', 'Scotland'),
        ('PL', 'Plymouth', 'South West'),
        ('PO', 'Portsmouth', 'South East'),
        ('PR', 'Preston', 'North West'),
        ('RG', 'Reading', 'South East'),
        ('RH', 'Redhill', 'South East'),
        ('RM', 'Romford', 'London'),
        ('S', 'Sheffield', 'Yorkshire and the Humber'),
        ('SA', 'Swansea', 'Wales'),
        ('SE', 'South East London', 'London'),
        ('SG', 'Stevenage', 'East of England'),
        ('SK', 'Stockport', 'North West'),
        ('SL', 'Slough', 'South East'),
        ('SM', 'Sutton', 'London'),
        ('SN', 'Swindon', 'South West'),
        ('SO', 'Southampton', 'South West'),
        ('SP', 'Salisbury', 'South West'),
        ('SR', 'Sunderland', 'North East'),
        ('SS', 'Southend-on-Sea', 'East of England'),
        ('ST', 'Stoke-on-Trent', 'West Midlands'),
        ('SW', 'South West London', 'London'),
        ('SY', 'Shrewsbury', 'Wales'),
        ('TA', 'Taunton', 'South West'),
        ('TD', 'Galashiels', 'Scotland'),
        ('TF', 'Telford', 'West Midlands'),
        ('TN', 'Tunbridge Wells', 'South East'),
        ('TQ', 'Torquay', 'South West'),
        ('TR', 'Truro', 'South West'),
        ('TS', 'Cleveland', 'North East'),
        ('TW', 'Twickenham', 'London'),
        ('UB', 'Southall', 'London'),
        ('W', 'West London', 'London'),
        ('WA', 'Warrington', 'North West'),
        ('WC', 'West Central London', 'London'),
        ('WD', 'Watford', 'East of England'),
        ('WF', 'Wakefield', 'Yorkshire'),
        ('WN', 'Wigan', 'North West'),
        ('WR', 'Worcester', 'West Midlands'),
        ('WS', 'Walsall', 'West Midlands'),
        ('WV', 'Wolverhampton', 'West Midlands'),
        ('YO', 'York', 'Yorkshire'),
        ('ZE', 'Shetland', 'Scotland');
    
    -- Update some approximate center coordinates for major areas
    UPDATE [dbo].[PostcodeAreas]
    SET CenterLatitude = CASE PostcodeArea
        WHEN 'AB' THEN 57.1497
        WHEN 'B' THEN 52.4862
        WHEN 'BA' THEN 51.3811
        WHEN 'BD' THEN 53.7958
        WHEN 'BH' THEN 50.7192
        WHEN 'BN' THEN 50.8225
        WHEN 'BR' THEN 51.4051
        WHEN 'BS' THEN 51.4545
        WHEN 'CF' THEN 51.4816
        WHEN 'CH' THEN 53.1905
        WHEN 'CM' THEN 51.7356
        WHEN 'CR' THEN 51.3762
        WHEN 'E' THEN 51.5423
        WHEN 'EC' THEN 51.5155
        WHEN 'EH' THEN 55.9533
        WHEN 'EX' THEN 50.7184
        WHEN 'G' THEN 55.8642
        WHEN 'GL' THEN 51.8642
        WHEN 'GU' THEN 51.2371
        WHEN 'L' THEN 53.4084
        WHEN 'LE' THEN 52.6369
        WHEN 'LS' THEN 53.8008
        WHEN 'M' THEN 53.4808
        WHEN 'MK' THEN 52.0406
        WHEN 'N' THEN 51.5654
        WHEN 'NE' THEN 54.9783
        WHEN 'NG' THEN 52.9548
        WHEN 'NW' THEN 51.5424
        WHEN 'OX' THEN 51.7520
        WHEN 'PL' THEN 50.3755
        WHEN 'PO' THEN 50.8198
        WHEN 'RG' THEN 51.4543
        WHEN 'S' THEN 53.3811
        WHEN 'SE' THEN 51.4709
        WHEN 'SO' THEN 50.9097
        WHEN 'SW' THEN 51.4994
        WHEN 'W' THEN 51.5074
        WHEN 'WC' THEN 51.5200
        ELSE NULL
    END,
    CenterLongitude = CASE PostcodeArea
        WHEN 'AB' THEN -2.0943
        WHEN 'B' THEN -1.8904
        WHEN 'BA' THEN -2.3522
        WHEN 'BD' THEN -1.7594
        WHEN 'BH' THEN -1.8808
        WHEN 'BN' THEN -0.1372
        WHEN 'BR' THEN 0.0135
        WHEN 'BS' THEN -2.5879
        WHEN 'CF' THEN -3.1791
        WHEN 'CH' THEN -2.8900
        WHEN 'CM' THEN 0.4691
        WHEN 'CR' THEN -0.0983
        WHEN 'E' THEN -0.0481
        WHEN 'EC' THEN -0.0915
        WHEN 'EH' THEN -3.1883
        WHEN 'EX' THEN -3.5339
        WHEN 'G' THEN -4.2518
        WHEN 'GL' THEN -2.2431
        WHEN 'GU' THEN -0.5749
        WHEN 'L' THEN -2.9916
        WHEN 'LE' THEN -1.1398
        WHEN 'LS' THEN -1.5491
        WHEN 'M' THEN -2.2426
        WHEN 'MK' THEN -0.7595
        WHEN 'N' THEN -0.1337
        WHEN 'NE' THEN -1.6078
        WHEN 'NG' THEN -1.1581
        WHEN 'NW' THEN -0.2000
        WHEN 'OX' THEN -1.2577
        WHEN 'PL' THEN -4.1427
        WHEN 'PO' THEN -1.0917
        WHEN 'RG' THEN -0.9781
        WHEN 'S' THEN -1.4701
        WHEN 'SE' THEN -0.0490
        WHEN 'SO' THEN -1.4044
        WHEN 'SW' THEN -0.1778
        WHEN 'W' THEN -0.1795
        WHEN 'WC' THEN -0.1246
        ELSE NULL
    END;
    
    COMMIT TRANSACTION;
    
    -- Report results
    DECLARE @AreaCount INT = (SELECT COUNT(*) FROM [dbo].[PostcodeAreas]);
    
    PRINT '';
    PRINT '=== Postcode Area Seed Complete ===';
    PRINT 'Total Areas Inserted: ' + CAST(@AreaCount AS VARCHAR(10));
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Error occurred during seeding:';
    PRINT ERROR_MESSAGE();
    THROW;
END CATCH;
GO