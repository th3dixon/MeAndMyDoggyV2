// Simple icon generator using canvas
// This creates placeholder icons for the Me and My Doggy app

const fs = require('fs');
const { createCanvas } = require('canvas');

// If canvas module is not installed, you can install it with:
// npm install canvas

function createIcon(size) {
    const canvas = createCanvas(size, size);
    const ctx = canvas.getContext('2d');
    
    // White background
    ctx.fillStyle = '#FFFFFF';
    ctx.fillRect(0, 0, size, size);
    
    // Orange circle
    const padding = size * 0.1;
    const circleRadius = (size - padding * 2) / 2;
    
    ctx.fillStyle = '#FF8C42';
    ctx.beginPath();
    ctx.arc(size / 2, size / 2, circleRadius, 0, Math.PI * 2);
    ctx.fill();
    
    // White paw print
    ctx.fillStyle = '#FFFFFF';
    const scale = size / 512; // Scale based on 512px reference
    
    // Main pad
    ctx.beginPath();
    ctx.ellipse(256 * scale, 290 * scale, 80 * scale, 70 * scale, 0, 0, Math.PI * 2);
    ctx.fill();
    
    // Toes
    const toes = [
        { x: 200, y: 200, r: 35 },
        { x: 256, y: 180, r: 35 },
        { x: 312, y: 200, r: 35 },
        { x: 170, y: 240, r: 30 },
        { x: 342, y: 240, r: 30 }
    ];
    
    toes.forEach(toe => {
        ctx.beginPath();
        ctx.arc(toe.x * scale, toe.y * scale, toe.r * scale, 0, Math.PI * 2);
        ctx.fill();
    });
    
    // Heart accent for larger sizes
    if (size >= 128) {
        ctx.fillStyle = '#EC4899';
        ctx.globalAlpha = 0.8;
        ctx.beginPath();
        ctx.moveTo(380 * scale, 100 * scale);
        // Simplified heart shape
        ctx.bezierCurveTo(
            380 * scale, 80 * scale,
            360 * scale, 60 * scale,
            340 * scale, 80 * scale
        );
        ctx.bezierCurveTo(
            320 * scale, 100 * scale,
            340 * scale, 140 * scale,
            340 * scale, 140 * scale
        );
        ctx.bezierCurveTo(
            340 * scale, 140 * scale,
            360 * scale, 100 * scale,
            380 * scale, 80 * scale
        );
        ctx.bezierCurveTo(
            400 * scale, 60 * scale,
            380 * scale, 80 * scale,
            380 * scale, 100 * scale
        );
        ctx.fill();
        ctx.globalAlpha = 1;
    }
    
    return canvas.toBuffer('image/png');
}

// Icon sizes to generate
const icons = [
    { filename: 'favicon-16x16.png', size: 16 },
    { filename: 'favicon-32x32.png', size: 32 },
    { filename: 'apple-touch-icon.png', size: 180 },
    { filename: 'android-chrome-192x192.png', size: 192 },
    { filename: 'android-chrome-512x512.png', size: 512 }
];

// Generate each icon
icons.forEach(({ filename, size }) => {
    try {
        const buffer = createIcon(size);
        fs.writeFileSync(filename, buffer);
        console.log(`Created ${filename}`);
    } catch (error) {
        console.error(`Error creating ${filename}:`, error.message);
    }
});

console.log('Icon generation complete!');
console.log('Note: If canvas module is not installed, run: npm install canvas');