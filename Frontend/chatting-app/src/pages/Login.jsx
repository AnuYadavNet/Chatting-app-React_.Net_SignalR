import React, { useState } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import api from '../services/api';
import '../App.css'; // Optional: Use app CSS for styling but will override with inline for speed here

export default function Login() {
    const [username, setUsername] = useState('');
    const [password, setPassword] = useState('');
    const [error, setError] = useState('');
    const [loading, setLoading] = useState(false);
    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        setError('');
        setLoading(true);

        try {
            const response = await api.post('/api/auth/login', { username, password });
            localStorage.setItem('jwtToken', response.data.token);
            localStorage.setItem('username', response.data.username);
            navigate('/');
        } catch (err) {
            setError(err.response?.data?.error || 'Login failed. Please try again.');
        } finally {
            setLoading(false);
        }
    };

    return (
        <div style={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh', background: '#f3f4f6' }}>
            <div style={{ background: 'white', padding: '2rem', borderRadius: '8px', boxShadow: '0 4px 6px rgba(0,0,0,0.1)', width: '100%', maxWidth: '400px' }}>
                <h2 style={{ textAlign: 'center', marginBottom: '1.5rem', color: '#111827' }}>Sign In</h2>
                {error && <div style={{ background: '#fee2e2', color: '#b91c1c', padding: '0.5rem', borderRadius: '4px', marginBottom: '1rem', textAlign: 'center' }}>{error}</div>}
                <form onSubmit={handleLogin}>
                    <div style={{ marginBottom: '1rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.5rem', color: '#374151' }}>Username</label>
                        <input type="text" value={username} onChange={e => setUsername(e.target.value)} required style={{ width: '100%', padding: '0.75rem', borderRadius: '4px', border: '1px solid #d1d5db', boxSizing: 'border-box' }} />
                    </div>
                    <div style={{ marginBottom: '1.5rem' }}>
                        <label style={{ display: 'block', marginBottom: '0.5rem', color: '#374151' }}>Password</label>
                        <input type="password" value={password} onChange={e => setPassword(e.target.value)} required style={{ width: '100%', padding: '0.75rem', borderRadius: '4px', border: '1px solid #d1d5db', boxSizing: 'border-box' }} />
                    </div>
                    <button type="submit" disabled={loading} style={{ width: '100%', padding: '0.75rem', background: '#0ea5e9', color: 'white', border: 'none', borderRadius: '4px', fontSize: '1rem', cursor: loading ? 'not-allowed' : 'pointer', fontWeight: 600 }}>
                        {loading ? 'Signing in...' : 'Sign In'}
                    </button>
                    <div style={{ textAlign: 'center', marginTop: '1rem', fontSize: '0.875rem' }}>
                        Don't have an account? <Link to="/register" style={{ color: '#0ea5e9', textDecoration: 'none' }}>Register here</Link>
                    </div>
                </form>
            </div>
        </div>
    );
}
